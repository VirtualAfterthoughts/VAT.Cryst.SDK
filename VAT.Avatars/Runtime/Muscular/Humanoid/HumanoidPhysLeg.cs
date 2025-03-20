using UnityEngine;

using VAT.Avatars.Proportions;

using VAT.Shared.Data;

using static Unity.Mathematics.math;

namespace VAT.Avatars.Muscular
{
    using System;
    using Unity.Mathematics;
    using VAT.Avatars.Bones;
    using VAT.Cryst.Delegates;
    using VAT.Input;

    public class HumanoidPhysLeg : HumanoidPhysBoneGroup, IHumanLeg
    {
        public bool isLeft = false;

        public override int BoneCount => 3;

        public HumanoidPhysBone Hip => Bones[0] as HumanoidPhysBone;
        public HumanoidPhysBone Knee => Bones[1] as HumanoidPhysBone;
        public HumanoidPhysBone Ankle => Bones[2] as HumanoidPhysBone;

        IBone ILegGroup.Hip => Hip;

        IBone ILegGroup.Knee => Knee;

        IBone ILegGroup.Ankle => Ankle;

        private RelativeBone _relativeToe = null;
        public IBone Toe => _relativeToe;

        public Handedness Handedness => isLeft ? Handedness.LEFT : Handedness.RIGHT;

        public SimpleTransform EndTarget => _leg.EndTarget;

        private IHumanLeg _leg;

        public event Action<Vector3> OnStep
        {
            add
            {
                _leg.OnStep += value;
            }
            remove
            {
                _leg.OnStep -= value;
            }
        }

        public event TargetProcessorCallback OnProcessTarget
        {
            add
            {
                _leg.OnProcessTarget += value;
            }
            remove
            {
                _leg.OnProcessTarget -= value;
            }
        }

        public override void Initiate()
        {
            base.Initiate();

            string prefix = isLeft ? "Left" : "Right";
            _bones[0] = new HumanoidPhysBone($"{prefix} Hip", null, JointAngularLimits.Free);
            _bones[1] = new HumanoidPhysBone($"{prefix} Knee", Hip, JointAngularLimits.Free);
            _bones[2] = new HumanoidPhysBone($"{prefix} Ankle", Knee, JointAngularLimits.Free);

            // Joint limits
            var kneeJoint = Knee.ConfigurableJoint.ConfigurableJoint;
            kneeJoint.angularXMotion = kneeJoint.angularYMotion = kneeJoint.angularZMotion = ConfigurableJointMotion.Limited;
            kneeJoint.lowAngularXLimit = new SoftJointLimit() { limit = -130f };
        }

        public override void Solve()
        {
            var hipTarget = Hip.Parent.TransformBone(_leg.Hip.Parent, _leg.Hip);
            Hip.Solve(hipTarget);

            Hip.SetConnectedAnchor(hipTarget.Position);

            var kneeTarget = Hip.TransformBone(_leg.Hip, _leg.Knee);
            Knee.Solve(kneeTarget);

            Knee.SetConnectedAnchor(kneeTarget.Position);

            var ankleTarget = Knee.TransformBone(_leg.Knee, _leg.Ankle);
            Ankle.Solve(ankleTarget);

            Ankle.SetConnectedAnchor(ankleTarget.Position);
        }

        public override void Attach(PhysBoneGroup group)
        {
            FirstBone.Parent = group.FirstBone;

            // Joint limits
            var hipJoint = Hip.ConfigurableJoint.ConfigurableJoint;
            hipJoint.angularXMotion = hipJoint.angularYMotion = hipJoint.angularZMotion = ConfigurableJointMotion.Limited;
            hipJoint.lowAngularXLimit = new SoftJointLimit() { limit = -70f };
            hipJoint.highAngularXLimit = new SoftJointLimit() { limit = 160f };
            hipJoint.angularZLimit = new SoftJointLimit() { limit = 90f };
            hipJoint.angularYLimit = new SoftJointLimit() { limit = 60f };
        }

        public void WriteProportions(HumanoidLegProportions proportions)
        {
            Hip.SetMesh(GenerateHipMesh(proportions));
            Knee.SetMesh(GenerateKneeMesh(proportions));
            Ankle.SetMesh(GenerateAnkleMesh(proportions));

            Hip.Rigidbody.Rigidbody.detectCollisions = false;
            Knee.Rigidbody.Rigidbody.detectCollisions = false;
            Ankle.Rigidbody.Rigidbody.detectCollisions = false;
        }

        public void MatchPose(IHumanLeg leg)
        {
            _leg = leg;

            Hip.MatchBone(leg.Hip);
            Knee.MatchBone(leg.Knee);
            Ankle.MatchBone(leg.Ankle);

            Hip.ConfigurableJoint.ConfigurableJoint.connectedMassScale = 0f;

            _relativeToe = new RelativeBone(Ankle, leg.Ankle, leg.Toe);
        }

        public Mesh GenerateHipMesh(HumanoidLegProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var hip = proportions.hipEllipsoid.Convert<Ellipse>();
            var knee = proportions.kneeEllipsoid.Convert<Ellipse>();

            // Create hip -> knee
            EllipseCylinderMesh cylinder = new()
            {
                bottom = knee,
                bottomTransform = new SimpleTransform(back() * proportions.kneeOffsetZ + down() * proportions.hipEllipsoid.height, quaternion.identity),

                top = hip,
                topTransform = new SimpleTransform(float3.zero, quaternion.identity),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }

        public Mesh GenerateKneeMesh(HumanoidLegProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var knee = proportions.kneeEllipsoid.Convert<Ellipse>();
            var ankle = proportions.ankleEllipsoid.Convert<Ellipse>();

            // Create knee -> ankle
            EllipseCylinderMesh cylinder = new()
            {
                bottom = ankle,
                bottomTransform = new SimpleTransform(back() * proportions.ankleOffsetZ + down() * proportions.kneeEllipsoid.height, quaternion.identity),

                top = knee,
                topTransform = new SimpleTransform(float3.zero, quaternion.identity),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }

        public Mesh GenerateAnkleMesh(HumanoidLegProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var ankle = proportions.ankleEllipsoid.Convert<Ellipse>();
            var toe = proportions.toeEllipsoid.Convert<Ellipse>();

            float mult = isLeft ? -1f : 1f;

            float3 toeBottom = new float3(mult * proportions.toeOffset.x, -proportions.toeEllipsoid.height * 0.5f + -proportions.ankleEllipsoid.height + proportions.toeOffset.y, proportions.toeOffset.z);

            float toeEnd = toeBottom.z + toe.radius.y;
            toe.radius.y = toeEnd * 0.6f;
            toeBottom.z *= 0.5f;
            toeBottom.x *= 0.5f;

            Quaternion toeRotation = Quaternion.AngleAxis(mult * 5f, Vector3.up);

            // Create ankle -> toe
            EllipseCylinderMesh cylinder = new()
            {
                bottom = toe,
                bottomTransform = new SimpleTransform(toeBottom, toeRotation),

                top = ankle,
                topTransform = new SimpleTransform(float3.zero, quaternion.identity),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }
    }
}
