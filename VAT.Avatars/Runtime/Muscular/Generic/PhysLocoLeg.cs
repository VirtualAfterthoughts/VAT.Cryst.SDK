using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using VAT.Avatars.Skeletal;

using VAT.Cryst.Interfaces;
using VAT.Entities;

using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Avatars.Muscular
{
    public class PhysLocoLeg : PhysBoneGroupT<RigidbodyPhysBone>, IPoseableT<LocoLeg>
    {
        public override int BoneCount => 3;

        public RigidbodyPhysBone Knee => Bones[0];
        public RigidbodyPhysBone Fender => Bones[1];
        public RigidbodyPhysBone Foot => Bones[2];

        public PhysBone _pivot;
        public DataBone _pivotData;

        private LocoLeg _leg;

        private SphereCollider _locoBall;

        public Vector3 _targetVelocity = Vector3.forward * 0f;

        public override void Initiate()
        {
            base.Initiate();

            _bones[0] = new RigidbodyPhysBone("Knee", null, JointAngularLimits.Free);
            _bones[1] = new RigidbodyPhysBone("Fender", Knee, JointAngularLimits.Free);
            _bones[2] = new RigidbodyPhysBone("Foot", Fender, JointAngularLimits.Free);

            Foot.Rigidbody.Rigidbody.angularDrag = 0f;
            _locoBall = Foot.UnityGameObject.AddComponent<SphereCollider>();
            _locoBall.radius = 0.2f;

            var physMaterial = new PhysicMaterial("LocoSphere")
            {
                dynamicFriction = 2f,
                staticFriction = 2f,
                frictionCombine = PhysicMaterialCombine.Maximum,
                bounceCombine = PhysicMaterialCombine.Minimum
            };

            _locoBall.material = physMaterial;

            Foot.InsertCollider(_locoBall);
        }

        private float3 _lastDistance;

        public override void Solve()
        {
            var kneeTarget = Knee.Parent.TransformBone(_leg.Knee.Parent, _leg.Knee);
            Knee.Solve(kneeTarget);

            var dataTransform = SimpleTransform.Create(_pivotData.position, _leg.Knee.rotation);
            var pivotTransform = SimpleTransform.Create(_pivot.Transform.position, Knee.Transform.rotation);
            
            var position = pivotTransform.TransformPoint(dataTransform.InverseTransformPoint(_leg.Knee.position));

            var dataKnee = _leg.Knee.Transform;
            var physKnee = Knee.Transform;
            var distance = dataKnee.InverseTransformDirection(_pivotData.Transform.position - _leg.Foot.Transform.position);
            distance.y = 0f;
            distance = dataKnee.TransformDirection(distance);

            var footVelocity = physKnee.TransformDirection(dataKnee.InverseTransformDirection(PhysicsExtensions.GetLinearVelocity(_lastDistance, distance)));
            _lastDistance = distance;

            Knee.SetConnectedAnchor(position);

            var footTarget = Knee.TransformBone(_leg.Knee, _leg.Foot);
            Fender.Solve(footTarget);

            var space = Fender.Joint.JointSpace;
            var previousTarget = space.RawTargetPosition;
            var currentTarget = space.InverseTransformTargetPosition(footTarget.position, CrystSpace.WORLD);

            space.RawTargetPosition = currentTarget;
            space.RawTargetVelocity = PhysicsExtensions.GetLinearVelocity(previousTarget, currentTarget);

            var vel = _leg.Knee.Transform.InverseTransformDirection(_leg.velocity);
            vel.y = 0f;

            _targetVelocity = Knee.Transform.TransformDirection(vel) - footVelocity;

            // Ball torque
            float radius = _locoBall.radius;
            float3 targetAngularVelocity = new Vector3(_targetVelocity.z, _targetVelocity.y, -_targetVelocity.x) / radius;

            float frequency = 1f;
            float damping = 900f;
            float kp = (6f * frequency) * (6f * frequency) * 0.25f;
            float kd = 4.5f * frequency * damping;
            float dt = Time.fixedDeltaTime;
            float g = 1 / (1 + kd * dt + kp * dt * dt);
            float kdg = (kd + kp * dt) * g;
            Vector3 pidv = kdg * (targetAngularVelocity - Foot.Body.AngularVelocity);
            Quaternion rotInertia2World = Foot.Rigidbody.Rigidbody.inertiaTensorRotation * Foot.Transform.rotation;
            pidv = Quaternion.Inverse(rotInertia2World) * pidv;
            pidv.Scale(Foot.Rigidbody.Rigidbody.inertiaTensor);
            pidv = rotInertia2World * pidv;
            Foot.Body.AddTorque(pidv);
        }

        public void MatchPose(LocoLeg leg) {
            _leg = leg;

            Knee.MatchBone(leg.Knee);
            Fender.MatchBone(leg.Foot);
            Foot.MatchBone(leg.Foot);

            Knee.SetMass(8f);
            Fender.SetMass(16f);
            Foot.SetMass(16f);
            Foot.Rigidbody.Rigidbody.inertiaTensor = Vector3.one * 10f;

            Knee.ConfigurableJoint.ConfigurableJoint.rotationDriveMode = RotationDriveMode.Slerp;
            Knee.ConfigurableJoint.ConfigurableJoint.slerpDrive = new JointDrive()
            {
                positionSpring = 5e+06f,
                positionDamper = 1e+05f,
                maximumForce = 5e+06f
            };

            Fender.ConfigurableJoint.ConfigurableJoint.SetJointMotion(ConfigurableJointMotion.Locked, ConfigurableJointMotion.Locked);
            Fender.ConfigurableJoint.ConfigurableJoint.yMotion = ConfigurableJointMotion.Limited;
            Fender.ConfigurableJoint.ConfigurableJoint.linearLimit = new SoftJointLimit() { limit = _leg.Length * 0.5f };
            Fender.ConfigurableJoint.ConfigurableJoint.yDrive = new JointDrive()
            {
                positionSpring = 900000f,
                positionDamper = 200000f,
                maximumForce = 6000f,
            };
        }

        public override void ResetAnchors() {
            Knee.ResetAnchors();

            Fender.ResetAnchors((Knee.Transform.position + Foot.Transform.position) * 0.5f);
            Fender.SetAnchor(Fender.Transform.position);

            Foot.ResetAnchors();
            Foot.SetConnectedAnchor(Fender.Transform.position + Fender.Transform.up * 0.2f);
        }

        public override void Attach(PhysBoneGroup group) {
            FirstBone.Parent = group.FirstBone;
        }

        public float3 GetCenterOfPressure() {
            return Fender.Transform.position;
        }
    }
}
