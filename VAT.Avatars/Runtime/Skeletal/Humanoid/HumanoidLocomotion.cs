using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Nervous;
using VAT.Avatars.Proportions;

using VAT.Shared.Data;
using VAT.Shared.Extensions;
using static Unity.Mathematics.math;

namespace VAT.Avatars.Skeletal
{
    using System;
    using Unity.Mathematics;
    using UnityEngine.UIElements;
    using VAT.Avatars.Integumentary;

    public sealed class HumanoidLocomotion {
        private DataBone _feetCenter;
        public DataBone FeetCenter => _feetCenter;

        private HumanoidLocomotor[] _locomotors;
        public HumanoidLocomotor[] Locomotors => _locomotors;

        private DataBone _l1Vertebra;
        private DataBone _sacrum;

        private HumanoidProportions _proportions;

        private IAvatarPayload _payload;

        public void Initiate(DataBone sacrum, DataBone l1Vertebra) {
            _l1Vertebra = l1Vertebra;
            _sacrum = sacrum;

            _feetCenter = new DataBone(_sacrum);
        }

        public float3 GetLocomotorCenter()
        {
            float3 center = 0f;

            foreach (var locomotor in _locomotors)
            {
                center += locomotor.Result.position;
            }

            center /= _locomotors.Length;

            return center;
        }

        public void WriteProportions(HumanoidProportions proportions) {
            _proportions = proportions;

            // Note: may be replaced with not hard coded leg count.
            int count = 2;
            _locomotors = new HumanoidLocomotor[count];
            
            for (var i = 0; i < count; i++) {
                _locomotors[i] = new HumanoidLocomotor();
            }

            _locomotors[0].Initiate(proportions.leftLegProportions, true);
            _locomotors[1].Initiate(proportions.rightLegProportions, false);
        }

        public void Write(IAvatarPayload payload) {
            _payload = payload;
        }

        private Vector3 _lastFeetCenter;

        public void Solve(SimpleTransform root, float3 velocity = default) {
            // Position the feet center
            float feetAngle = Vector3.Angle(root.up, _sacrum.up);
            Vector3 feetAxis = Vector3.Cross(root.up, _sacrum.up);

            _feetCenter.position = Vector3.ProjectOnPlane(_l1Vertebra.position - root.position, root.up) + (Vector3)root.position;
            _feetCenter.rotation = Quaternion.AngleAxis(-feetAngle, feetAxis) * _sacrum.rotation;

            var postFeetPos = _feetCenter.position;
            velocity = PhysicsExtensions.GetLinearVelocity(_lastFeetCenter, postFeetPos);
            _lastFeetCenter = postFeetPos;

            // Presolve the locomotors
            for (var i = 0; i < Locomotors.Length; i++) {
                Locomotors[i].PreSolve(_sacrum.Transform, _feetCenter.Transform, velocity);
            }

            bool canStep = true;
            foreach (var locomotor in Locomotors)
                if (locomotor.IsThreshold) canStep = false;

            if (canStep) {
                int stepIndex = -1;
                float bestValue = -Mathf.Infinity;
                float bestAngle = -Mathf.Infinity;

                for (int i = 0; i < Locomotors.Length; i++)
                {
                    var locomotor = Locomotors[i];
                    if (locomotor.Stepping || !locomotor.CanStep)
                        continue;

                    float stepDistance = Vector3.Distance(locomotor.Result.position, locomotor.Resting.position);
                    float stepAngle = Quaternion.Angle(locomotor.Result.rotation, locomotor.Resting.rotation);

                    if (stepDistance > 0.1f * locomotor._legMultiplier)
                    {
                        if (stepDistance > bestValue)
                        {
                            stepIndex = i;
                            bestValue = stepDistance;
                        }
                    }
                    else if (stepAngle > 55f) {
                        if (stepAngle > bestAngle) {
                            stepIndex = i;
                            bestAngle = stepAngle;
                        }
                    }
                }

                if (stepIndex != -1)
                    Locomotors[stepIndex].Step();
            }

            // Now, solve the footstepping logic
            for (var i = 0; i < Locomotors.Length; i++) {
                Locomotors[i].Solve();
            }
        }
    }

    public sealed class HumanoidLocomotor
    {
        public static AnimationCurve ProgressCurve = new(new(0f, 0f, 2f, 2f, 0f, 0.33f), new(0.6f, 1.2f, 2f, -0.44f, 0.33f, 0.25f), new(1f, 1f, -0.5f, -0.5f, 0.33f, 0f));
        public static AnimationCurve StepCurve = new(new(0f, 0f, 0.04f, 0.04f, 0f, 0.3f), new(0.25f, 0.5f, 0.003f, 0.003f, 0.5f, 0.8f), new(0.5f, 0.4f, -0.9f, -0.9f, 0.34f, 0.5f), new(1f, 0f, 0.05f, 0.05f, 0.45f, 0f));

        private HumanoidLegProportions _proportions;
        private float _legLength;
        public float _legMultiplier;
        private bool _isLeft;

        private float _hipOffset;

        private Vector3 _velocity;
        private Vector3 _velocityAtStep;

        private quaternion _lastFeetRotation = quaternion.identity;
        private SimpleTransform _feetCenter = SimpleTransform.Default;
        private SimpleTransform _sacrum = SimpleTransform.Default;

        private SimpleTransform _resting = SimpleTransform.Default;
        public SimpleTransform Resting => _resting;

        private SimpleTransform _localResult = SimpleTransform.Default;
        private SimpleTransform _result = SimpleTransform.Default;
        public SimpleTransform Result => _result;

        public event Action<Vector3> OnStep;

        private SimpleTransform _stepFrom = SimpleTransform.Default;
        private SimpleTransform _stepTo = SimpleTransform.Default;

        private bool _steppedOnce = false;

        private bool _stepping = false;
        public bool Stepping => _stepping;

        private readonly AnimationCurve _heelHeightCurve = new(new Keyframe(0f, 0f, 0.05f, 0.05f), new Keyframe(0.2f, 1f, 4.082311f, 4.082311f, 0.2641137f, 0.0278182f), new(0.7f, 0.5f), new Keyframe(1f, 0f, 0.01742062f, 0.01742062f, 0.6331643f, 0f));

        private float _stepSpeed = 1.42f;

        private float _targetStepTime = 0f;
        private float _stepTime = 0f;
        public float StepPercent => _stepTime / _targetStepTime;

        private float _threshold = 0.55f;
        public bool IsThreshold => (Stepping && StepPercent < _threshold);

        private bool _isGrounded = true;
        public bool CanStep => true;

        public void Initiate(HumanoidLegProportions proportions, bool isLeft) {
            _proportions = proportions;
            _legLength = _proportions.GetLength();
            _legMultiplier = _legLength / 0.84f;

            _isLeft = isLeft;

            float mult = _isLeft ? -1f : 1f;

            _hipOffset = mult * _proportions.hipSeparationOffset;
        }

        private Vector3 _groundNormal = Vector3.up;

        public void PreSolve(SimpleTransform sacrum, SimpleTransform feetCenter, Vector3 velocity) {
            feetCenter.rotation = Quaternion.FromToRotation(feetCenter.up, _groundNormal) * feetCenter.rotation;

            _lastFeetRotation = _feetCenter.rotation;
            _feetCenter = feetCenter;

            quaternion difference = mul(_feetCenter.rotation, inverse(_lastFeetRotation));
            var correction = inverse(difference);

            _sacrum = sacrum;
            _result = feetCenter.Transform(_localResult);

            // Get the resting foot position and rotation
            float restOffset = _hipOffset * 1.5f;
            var rotation = feetCenter.rotation;
            _resting = SimpleTransform.Create(feetCenter.position + feetCenter.right * restOffset, rotation);

            if (!_steppedOnce)
            {
                _result = _resting;
                _steppedOnce = true;
            }

            // Ground check
            _isGrounded = false;
            var hits = Physics.RaycastAll(sacrum.position, _resting.position - sacrum.position, _proportions.GetLength() * 1.3f, ~0, QueryTriggerInteraction.Ignore);

            // TEMPORARY, replace with layermask or something else later
            RaycastHit? closestHit = null;

            foreach (var hit in hits)
            {
                var parent = hit.collider.transform.parent;
                if (parent != null && parent.name == "[Rig - Physics]")
                {
                    continue;
                }

                _isGrounded = true;
                
                if (!closestHit.HasValue)
                {
                    closestHit = hit;
                }
                else if (hit.distance < closestHit.Value.distance) { }
                {
                    closestHit = hit;
                }
            }

            if (closestHit.HasValue)
            {
                _groundNormal = closestHit.Value.normal;
            }
            else
            {
                _groundNormal = feetCenter.up;
            }

            // Zero velocity height relative to ground
            var worldToGround = Quaternion.FromToRotation(_groundNormal, Vector3.up);

            velocity = worldToGround * velocity;
            velocity.y = 0f;
            velocity = Quaternion.Inverse(worldToGround) * velocity;

            _velocity = velocity;

            if (_isGrounded)
            {
                _result.position = mul(correction, _result.position - feetCenter.position) + feetCenter.position;
                _result.rotation = mul(correction, _result.rotation);

                _result.position -= (float3)velocity * Time.deltaTime;
            }

            _result.position = ClampPosition(_result.position);

            _stepSpeed = Mathf.Lerp(0.8f, 1.1f, CalculateVelocityLerp(_velocity)) * _legMultiplier;
        }

        private float CalculateVelocityLerp(Vector3 velocity)
        {
            return velocity.magnitude / (4f * _legMultiplier);
        }

        private float3 ClampPosition(float3 position) {
            var feetCenter = _feetCenter;

            position = feetCenter.InverseTransformPoint(position);
            float3 extents = new(0.5f * _legMultiplier, 2f * _legMultiplier, 0.5f * _legMultiplier);
            position = clamp(position, -extents, extents);
            position = feetCenter.TransformPoint(position);

            return position;
        }

        public void Step() {
            _stepping = true;
            _stepTime = 0f;

            _velocityAtStep = _velocity;

            _threshold = Mathf.Lerp(0.9f, 0.6f, CalculateVelocityLerp(_velocityAtStep));

            var fromPos = ClampPosition(_result.position);
            var toPos = _resting.position + (float3)Vector3.ClampMagnitude(_legMultiplier * 0.01f * _velocityAtStep, 0.15f * _legMultiplier);

            _stepFrom = _feetCenter.InverseTransform(SimpleTransform.Create(fromPos, _result.rotation));
            _stepTo = _feetCenter.InverseTransform(SimpleTransform.Create(toPos, _resting.rotation));

            var stepDist = distance(_stepFrom.position, _stepTo.position) + _legLength * 0.25f;
            var stepSp  = _stepSpeed;

            _targetStepTime = stepDist / stepSp;
        }

        public void Solve() 
        {
            if (_stepping) 
            {
                if (StepPercent >= 1f) 
                {
                    EndStep();
                }
                else 
                {
                    var stepFromWorld = _feetCenter.Transform(_stepFrom);
                    var stepToWorld = _feetCenter.Transform(_stepTo);

                    float lerp = ProgressCurve.Evaluate(StepPercent);
                    var pos = Vector3.LerpUnclamped(stepFromWorld.position, stepToWorld.position, lerp);
                    var rot = Quaternion.LerpUnclamped(stepFromWorld.rotation, stepToWorld.rotation, lerp);

                    float stepHeight = CurveFootHeight(StepPercent) * 0.15f * (min(length(_velocityAtStep * _legMultiplier) + 1f, 8f * _legMultiplier));
                    float max = distance(_sacrum.position, _feetCenter.position) * 0.66f;

                    stepHeight = (stepHeight + max - abs(stepHeight - max)) * 0.4f;
                    pos += _groundNormal * stepHeight;

                    if (stepHeight < 0.2f * _legMultiplier && _isGrounded) 
                    {
                        stepFromWorld.position -= (float3)_velocityAtStep * Time.deltaTime;
                    }

                    _stepFrom = _feetCenter.InverseTransform(stepFromWorld);

                    var right = mul(rot, math.right());
                    float maxAngle = Mathf.Lerp(45f, 80f, CalculateVelocityLerp(_velocityAtStep));
                    rot = Quaternion.AngleAxis(maxAngle * _heelHeightCurve.Evaluate(lerp), right) * rot;

                    _result = SimpleTransform.Create(pos, rot);

                    _stepTime += Time.deltaTime;
                }
            }

            _localResult = _feetCenter.InverseTransform(_result);
        }

        private float CurveFootHeight(float percent) {
            return StepCurve.Evaluate(percent);
        }

        private void EndStep() {
            _stepTime = 0f;
            _stepping = false;
            _result = _feetCenter.Transform(_stepTo);
            _localResult = _feetCenter.InverseTransform(_result);

            if (_isGrounded)
            {
                OnStep?.Invoke(_result.position);
            }
        }
    }
}
