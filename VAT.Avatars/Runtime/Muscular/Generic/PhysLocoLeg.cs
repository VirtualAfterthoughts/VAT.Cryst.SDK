using Unity.Mathematics;
using UnityEngine;

using VAT.Avatars.Bones;
using VAT.Avatars.Skeletal;

using VAT.Cryst.Math;
using VAT.Entities;
using VAT.Input.Data;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Avatars.Muscular
{
    public class PhysLocoLeg : PhysBoneGroupT<RigidbodyPhysBone>
    {
        public override int BoneCount => 3;

        public RigidbodyPhysBone Knee => Bones[0] as RigidbodyPhysBone;
        public RigidbodyPhysBone Fender => Bones[1] as RigidbodyPhysBone;
        public RigidbodyPhysBone Foot => Bones[2] as RigidbodyPhysBone;

        public IBone _pivot;
        public IBone _pivotData;

        private LocoLeg _leg;

        private SphereCollider _locoBall;
        private SphereCollider _fender;

        public Vector3 _targetVelocity = Vector3.forward * 0f;

        private Vector3 _fenderDebt = Vector3.zero;

        private float _radius = 0.2f;

        public override void Initiate()
        {
            base.Initiate();

            _bones[0] = new RigidbodyPhysBone("Knee", null, JointAngularLimits.Free);
            _bones[1] = new RigidbodyPhysBone("Fender", Knee, JointAngularLimits.Free);
            _bones[2] = new RigidbodyPhysBone("Foot", Fender, JointAngularLimits.Free);

            Foot.Rigidbody.Rigidbody.angularDrag = 0f;
            Foot.Rigidbody.Rigidbody.maxDepenetrationVelocity = 0.1f;

            _locoBall = Foot.UnityGameObject.AddComponent<SphereCollider>();

            var physMaterial = new PhysicMaterial("LocoSphere")
            {
                dynamicFriction = 8f,
                staticFriction = 8f,
                frictionCombine = PhysicMaterialCombine.Maximum,
                bounceCombine = PhysicMaterialCombine.Minimum
            };

            _locoBall.material = physMaterial;

            Foot.InsertCollider(_locoBall);

            _fender = Fender.UnityGameObject.AddComponent<SphereCollider>();

            var frictionless = new PhysicMaterial("Fender")
            {
                dynamicFriction = 0f,
                staticFriction = 0f,
                frictionCombine = PhysicMaterialCombine.Multiply,
                bounceCombine = PhysicMaterialCombine.Multiply
            };

            _fender.material = frictionless;

            Fender.InsertCollider(_fender);
        }

        private float3 _lastDistance;

        public void WriteProportions(BodyMeasurements measurements)
        {
            _radius = 0.2f * (measurements.height / 1.76f);

            _locoBall.radius = _radius;
            _fender.radius = _radius * 1.25f;
            _fender.center = _radius * 1.5f * Vector3.up;
        }

        private Vector3 _stairForce = Vector3.zero;

        public override void Solve()
        {
            float shrinkMult = (1f - _leg._footShrink);
            _locoBall.radius = _radius * shrinkMult;

            float fenderRadius = _radius * 1.25f * shrinkMult;

            var kneeTarget = Knee.Parent.TransformBone(_leg.Knee.Parent, _leg.Knee);

            Knee.Solve(kneeTarget);

            Knee.SetConnectedAnchor(kneeTarget.position);

            var dataKnee = _leg.Knee.Transform;
            var physKnee = Knee.Transform;
            var distance = dataKnee.InverseTransformDirection(_pivotData.Transform.position - _leg.Foot.Transform.position);
            distance.y = 0f;
            distance = dataKnee.TransformDirection(distance);

            var footVelocity = physKnee.TransformDirection(dataKnee.InverseTransformDirection(PhysicsExtensions.GetLinearVelocity(_lastDistance, distance)));
            _lastDistance = distance;

            footVelocity *= _leg._spineDebtMultiplier;

            var footTarget = Knee.TransformBone(_leg.Knee, _leg.Foot);
            Fender.Solve(footTarget);

            var space = Fender.Joint.JointSpace;
            var previousTarget = space.RawTargetPosition;
            var currentTarget = space.InverseTransformTargetPosition(footTarget.position, CrystSpace.WORLD);

            space.RawTargetPosition = currentTarget;

            _fenderDebt += (Vector3)PhysicsExtensions.GetLinearVelocity(previousTarget, currentTarget);

            var debt = _fenderDebt * 0.5f;
            _fenderDebt -= debt;
            space.RawTargetVelocity = debt * _leg._jumpMultiplier;

            var vel = _leg.Knee.Transform.InverseTransformDirection(_leg.velocity);
            vel.y = 0f;

            _targetVelocity = Knee.Transform.TransformDirection(vel) - footVelocity;

            StairSolve(ref fenderRadius);
            SlopeSolve(out var counterVelocity);

            _targetVelocity += counterVelocity;

            float lerp = Smoothing.CalculateDecay(32f, Time.deltaTime);
            _fender.radius = Mathf.Lerp(_fender.radius, fenderRadius, lerp);

            // Ball torque
            float radius = _locoBall.radius;
            float3 targetAngularVelocity = new Vector3(_targetVelocity.z, _targetVelocity.y, -_targetVelocity.x) / radius;

            float frequency = 1f;
            float damping = 1000f;
            float kp = (6f * frequency) * (6f * frequency) * 0.25f;
            float kd = 4.5f * frequency * damping;
            float dt = Time.fixedDeltaTime;
            float g = 1 / (1 + kd * dt + kp * dt * dt);
            float kdg = (kd + kp * dt) * g;

            Vector3 error = targetAngularVelocity - Foot.Body.AngularVelocity;

            Vector3 torque = kdg * error;

            Quaternion rotInertia2World = Foot.Rigidbody.Rigidbody.inertiaTensorRotation * Foot.Transform.rotation;
            torque = Quaternion.Inverse(rotInertia2World) * torque;
            torque.Scale(Foot.Rigidbody.Rigidbody.inertiaTensor);
            torque = rotInertia2World * torque;

            torque *= 2f;

            Foot.Body.AddTorque(torque);
        }

        private void SlopeSolve(out Vector3 counterVelocity)
        {
            counterVelocity = Vector3.zero;

            var worldDown = Physics.gravity.normalized;

            var raycast = Physics.Raycast(_locoBall.transform.position, worldDown, out var hitInfo, _locoBall.radius * 1.1f, ~0, QueryTriggerInteraction.Ignore);

            if (raycast)
            {
                var normal = hitInfo.normal;
                Vector3 gradient = Vector3.Cross(normal, worldDown);
                Vector3 uphill = Vector3.Cross(normal, gradient);

                var force = uphill * Physics.gravity.magnitude;

                counterVelocity = force * 0.005f;
            }
        }

        private void StairSolve(ref float fenderRadius)
        {
            var worldDown = Physics.gravity.normalized;

            float stepRadius = _locoBall.radius * 0.95f;
            var raycast = Physics.Raycast(_locoBall.transform.position - stepRadius * Vector3.up, _targetVelocity.normalized, out var hitInfo, fenderRadius * 1.4f, ~0, QueryTriggerInteraction.Ignore);

            if (raycast && Vector3.Angle(worldDown, hitInfo.normal) >= 70f && Vector3.Dot(_targetVelocity, -hitInfo.normal) >= 0.5f)
            {
                float totalHeight = _fender.center.y + fenderRadius;
                var stairUp = hitInfo.point + totalHeight * -worldDown;
                var stairCenter = stairUp - hitInfo.normal * 0.1f;

                var stairCast = Physics.Raycast(stairCenter, worldDown, out var stairHit, totalHeight * 0.9f, ~0, QueryTriggerInteraction.Ignore);

                if (stairCast && Vector3.Angle(stairHit.normal, hitInfo.normal) >= 70f)
                {
                    var stairDistance = Mathf.Clamp01(Vector3.Distance(hitInfo.point, stairHit.point) / _locoBall.radius);

                    _stairForce += 50f * stairDistance * _targetVelocity;

                    fenderRadius *= 1f - stairDistance;
                }
            }

            var stairDebt = _stairForce * 0.01f;
            _stairForce -= stairDebt;
            Foot.Rigidbody.AddForce(stairDebt, CrystForceMode.Acceleration);
        }

        public void MatchPose(LocoLeg leg)
        {
            _leg = leg;

            Knee.MatchBone(leg.Knee);
            Fender.MatchBone(leg.Foot);
            Foot.MatchBone(leg.Foot);

            float legLength = _leg.Length * 0.5f;
            float legScalar = legLength / 0.5f;

            Knee.SetMass(8f * legScalar);
            Fender.SetMass(16f * legScalar);
            Foot.SetMass(16f * legScalar);

            Knee.ConfigurableJoint.ConfigurableJoint.rotationDriveMode = RotationDriveMode.Slerp;

            float kneeForce = 5000000f * legScalar;
            float kneeDamper = kneeForce * 0.1f;
            float kneeMaxForce = kneeDamper * 0.05f;

            Knee.ConfigurableJoint.ConfigurableJoint.slerpDrive = new JointDrive()
            {
                positionSpring = kneeForce,
                positionDamper = kneeDamper,
                maximumForce = kneeMaxForce
            };

            Fender.ConfigurableJoint.ConfigurableJoint.SetJointMotion(ConfigurableJointMotion.Locked, ConfigurableJointMotion.Locked);
            Fender.ConfigurableJoint.ConfigurableJoint.yMotion = ConfigurableJointMotion.Limited;
            Fender.ConfigurableJoint.ConfigurableJoint.linearLimit = new SoftJointLimit() { limit = legLength * 1.1f };

            Fender.ConfigurableJoint.ConfigurableJoint.yDrive = new JointDrive()
            {
                positionSpring = 900000f,
                positionDamper = 200000f,
                maximumForce = 6000f * legScalar,
            };
        }

        public override void ResetAnchors()
        {
            Knee.ResetAnchors();

            Fender.ResetAnchors((Knee.Transform.position + Foot.Transform.position) * 0.5f);
            Fender.SetAnchor(Fender.Transform.position);

            Foot.ResetAnchors();
            Foot.SetConnectedAnchor(Fender.Transform.position + Fender.Transform.up * _radius);
        }

        public override void Attach(PhysBoneGroup group)
        {
            FirstBone.Parent = group.FirstBone;
        }

        public float3 GetCenterOfPressure()
        {
            return Fender.Transform.position;
        }
    }
}
