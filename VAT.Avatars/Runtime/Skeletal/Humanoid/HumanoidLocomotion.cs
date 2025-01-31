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
    using VAT.Cryst.Math;

    public sealed class HumanoidLocomotion
    {
        private DataBone _feetCenter;
        public DataBone FeetCenter => _feetCenter;

        private HumanoidLocomotor[] _locomotors;
        public HumanoidLocomotor[] Locomotors => _locomotors;

        private DataBone _l1Vertebra;
        private DataBone _sacrum;

        private HumanoidProportions _proportions;

        private IAvatarPayload _payload;

        public void Initiate(DataBone sacrum, DataBone l1Vertebra)
        {
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

        public void WriteProportions(HumanoidProportions proportions)
        {
            _proportions = proportions;

            // Note: may be replaced with not hard coded leg count.
            int count = 2;
            _locomotors = new HumanoidLocomotor[count];

            for (var i = 0; i < count; i++)
            {
                _locomotors[i] = new HumanoidLocomotor();
            }

            _locomotors[0].Initiate(proportions.leftLegProportions, true);
            _locomotors[1].Initiate(proportions.rightLegProportions, false);
        }

        public void Write(IAvatarPayload payload)
        {
            _payload = payload;
        }

        private Vector3 _lastFeetCenter;

        private Vector3 _lastVelocity;

        public void Solve(SimpleTransform root, SimpleTransform sacrum, float3 velocity = default)
        {
            // Position the feet center
            float feetAngle = Vector3.Angle(root.up, sacrum.up);
            Vector3 feetAxis = Vector3.Cross(root.up, sacrum.up);

            _feetCenter.position = Vector3.ProjectOnPlane(_l1Vertebra.position - root.position, root.up) + (Vector3)root.position;
            _feetCenter.rotation = Quaternion.AngleAxis(-feetAngle, feetAxis) * sacrum.rotation;

            var postFeetPos = _feetCenter.position;
            velocity = PhysicsExtensions.GetLinearVelocity(_lastFeetCenter, postFeetPos);
            _lastFeetCenter = postFeetPos;

            var acceleration = ((Vector3)velocity - _lastVelocity) / Time.fixedDeltaTime;

            if (acceleration.magnitude > 10000f)
            {
                velocity = Vector3.zero;
            }

            _lastVelocity = velocity;

            // Presolve the locomotors
            for (var i = 0; i < Locomotors.Length; i++)
            {
                Locomotors[i].PreSolve(sacrum, _feetCenter.Transform, velocity);
            }

            bool canStep = true;
            foreach (var locomotor in Locomotors)
                if (locomotor.IsThreshold) canStep = false;

            if (canStep)
            {
                int stepIndex = -1;
                float bestValue = -Mathf.Infinity;
                float bestAngle = -Mathf.Infinity;

                for (int i = 0; i < Locomotors.Length; i++)
                {
                    var locomotor = Locomotors[i];
                    if (!locomotor.CanStep)
                        continue;

                    float stepDistance = Vector3.Distance(locomotor.Result.position, locomotor.Resting.position);
                    float stepAngle = Quaternion.Angle(locomotor.Result.rotation, locomotor.Resting.rotation);

                    bool distanceCheck = stepDistance > locomotor._maxStepDistance * locomotor._legMultiplier && stepDistance > bestValue;
                    bool angleCheck = stepAngle > 55f && stepAngle > bestAngle;

                    if (distanceCheck || angleCheck)
                    {
                        stepIndex = i;
                        bestValue = stepDistance;
                        bestAngle = stepAngle;
                    }
                }

                if (stepIndex != -1)
                {
                    Locomotors[stepIndex].Step();
                }
            }

            // Now, solve the footstepping logic
            for (var i = 0; i < Locomotors.Length; i++)
            {
                Locomotors[i].Solve();
            }
        }
    }

    public sealed class HumanoidLocomotor
    {
        public static AnimationCurve ProgressCurve = new(new(0f, 0f, 2f, 2f, 0f, 0.33f), new(0.6f, 1.2f, 2f, -0.44f, 0.33f, 0.25f), new(1f, 1f, -0.5f, -0.5f, 0.33f, 0f));
        public static AnimationCurve StepCurve = new(new(0f, 0f, 0.04f, 0.04f, 0f, 0.3f), new(0.25f, 0.5f, 0.003f, 0.003f, 0.5f, 0.8f), new(0.5f, 0.4f, -0.9f, -0.9f, 0.34f, 0.5f), new(1f, 0f, 0.05f, 0.05f, 0.45f, 0f));

        private HumanoidLegProportions _proportions;
        public float _legLength;
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

        private float _stepSpeed = 1.42f;

        private float _targetStepTime = 0f;
        private float _stepTime = 0f;
        public float StepPercent => _stepTime / _targetStepTime;

        private float _threshold = 0.55f;
        public bool IsThreshold => (Stepping && StepPercent < _threshold);

        private bool _isGrounded = true;
        public bool CanStep => !Stepping || StepPercent > 0.75f;

        public float _maxStepDistance = 0.3f;

        private float _currentStepHeight = 0f;

        public float WeightSupport => 1f - Mathf.Clamp01(_currentStepHeight / _legLength);

        private float _maxSpeed = 4f;

        public void Initiate(HumanoidLegProportions proportions, bool isLeft)
        {
            _proportions = proportions;
            _legLength = _proportions.GetLength();
            _legMultiplier = _legLength / 0.84f;

            _isLeft = isLeft;

            float mult = _isLeft ? -1f : 1f;

            _hipOffset = mult * _proportions.hipSeparationOffset;
        }

        private Vector3 _groundNormal = Vector3.up;
        private Vector3 _groundVelocity = Vector3.zero;

        public void PreSolve(SimpleTransform sacrum, SimpleTransform feetCenter, Vector3 velocity)
        {
            var originalUp = feetCenter.up;
            feetCenter.rotation = Quaternion.FromToRotation(originalUp, _groundNormal) * feetCenter.rotation;

            _lastFeetRotation = _feetCenter.rotation;
            _feetCenter = feetCenter;

            quaternion difference = mul(_feetCenter.rotation, inverse(_lastFeetRotation));
            var correction = inverse(difference);

            _sacrum = sacrum;
            _result = feetCenter.Transform(_localResult);

            // Get the resting foot position and rotation
            float restOffset = _hipOffset * 1.2f;
            var rotation = feetCenter.rotation;
            _resting = SimpleTransform.Create(feetCenter.position + feetCenter.right * restOffset, rotation);

            if (!_steppedOnce)
            {
                _result = _resting;
                _steppedOnce = true;
            }

            // Ground check
            _isGrounded = false;
            var hits = Physics.RaycastAll(sacrum.position, _resting.position - sacrum.position, _proportions.GetLength() * 1.5f, ~0, QueryTriggerInteraction.Ignore);

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

            _groundVelocity = Vector3.zero;

            Vector3 newNormal;
            if (_currentStepHeight <= 0.1f * _legMultiplier && closestHit.HasValue && Vector3.Angle(originalUp, closestHit.Value.normal) <= 70f)
            {
                newNormal = closestHit.Value.normal;

                if (closestHit.Value.rigidbody)
                {
                    _groundVelocity = closestHit.Value.rigidbody.GetPointVelocity(closestHit.Value.point);
                }
            }
            else
            {
                newNormal = originalUp;
            }

            _groundNormal = Vector3.Slerp(_groundNormal, newNormal, Smoothing.CalculateDecay(18f, Time.deltaTime));

            // Zero velocity height relative to ground
            velocity -= _groundVelocity;

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

            float lerpedSpeed = Mathf.Lerp(0.5f, 1.8f, CalculateVelocityLerp(_velocity));
            float scaledSpeed = Mathf.Max(0.7f, lerpedSpeed) * _legMultiplier;
            _stepSpeed = Mathf.Lerp(_stepSpeed, scaledSpeed, Time.deltaTime * 24f);

            _maxStepDistance = 0.3f;
        }

        private float CalculateVelocityLerp(Vector3 velocity)
        {
            return Mathf.Clamp01(velocity.magnitude / _maxSpeed);
        }

        private float3 ClampPosition(float3 position)
        {
            var feetCenter = _feetCenter;

            position = feetCenter.InverseTransformPoint(position);
            float3 extents = new(0.5f * _legMultiplier, 2f * _legMultiplier, 0.5f * _legMultiplier);
            position = clamp(position, -extents, extents);
            position = feetCenter.TransformPoint(position);

            return position;
        }

        public void Step()
        {
            _stepping = true;
            _stepTime = 0f;
            _currentStepHeight = 0f;

            _velocityAtStep = _velocity;

            _threshold = 0.6f;

            var fromPos = ClampPosition(_result.position);

            var offsetVelocity = _velocityAtStep;
            offsetVelocity = _feetCenter.InverseTransformDirection(offsetVelocity);

            float sign = _isLeft ? -1f : 1f;

            if (Mathf.Sign(offsetVelocity.x) != sign)
            {
                offsetVelocity.x = 0f;
            }

            offsetVelocity = _feetCenter.TransformDirection(offsetVelocity);

            var velocityIncrease = Vector3.ClampMagnitude(offsetVelocity / _maxSpeed, 1f);
            var toOffset = (float3)(0.15f * velocityIncrease) * _legMultiplier;
            var toPos = _resting.position + toOffset;

            _stepFrom = _feetCenter.InverseTransform(SimpleTransform.Create(fromPos, _result.rotation));
            _stepTo = _feetCenter.InverseTransform(SimpleTransform.Create(toPos, _resting.rotation));

            var stepDist = distance(_stepFrom.position, _stepTo.position) + _legLength * 0.25f;
            var stepSp = _stepSpeed;

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

                    float stepHeight = CurveFootHeight(StepPercent) * 0.3f * (CalculateVelocityLerp(_velocityAtStep) + 1f) * _legLength;
                    float max = distance(_sacrum.position, _feetCenter.position) / _legLength;

                    stepHeight *= max;
                    pos += _groundNormal * stepHeight;

                    if (stepHeight < 0.3f * _legMultiplier && _isGrounded)
                    {
                        stepFromWorld.position -= (float3)_velocityAtStep * Time.deltaTime;
                    }

                    _currentStepHeight = stepHeight;

                    _stepFrom = _feetCenter.InverseTransform(stepFromWorld);

                    var right = mul(rot, math.right());
                    float maxAngle = 25f;

                    float heelHeight = Mathf.Clamp(-stepHeight / _legLength * 10f * ((StepPercent - 0.5f) * 2f), -1f, 1f);

                    float velLerp = CalculateVelocityLerp(_velocity);

                    if (StepPercent > 0.5f)
                    {
                        heelHeight *= Mathf.Lerp(3f, 1f, velLerp);
                    }
                    else
                    {
                        heelHeight *= Mathf.Lerp(2f, 2.5f, velLerp);
                    }

                    rot = Quaternion.AngleAxis(maxAngle * heelHeight, right) * rot;

                    _result = SimpleTransform.Create(pos, rot);

                    _stepTime += Time.deltaTime;
                }
            }

            _localResult = _feetCenter.InverseTransform(_result);
        }

        private float CurveFootHeight(float percent)
        {
            return StepCurve.Evaluate(percent);
        }

        private void EndStep()
        {
            _stepTime = 0f;
            _currentStepHeight = 0f;
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
