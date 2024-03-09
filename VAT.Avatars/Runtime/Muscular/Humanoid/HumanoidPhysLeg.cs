using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Proportions;
using VAT.Avatars.Constants;
using VAT.Avatars.Skeletal;

using VAT.Cryst.Interfaces;

using VAT.Shared.Data;

using static Unity.Mathematics.math;

namespace VAT.Avatars.Muscular
{
    using Unity.Mathematics;
    using VAT.Avatars.REWORK;

    public class HumanoidPhysLeg : HumanoidPhysBoneGroup, IPoseableT<IHumanLeg>, IHumanLeg
    {
        public bool isLeft = false;

        public override int BoneCount => 3;

        public HumanoidPhysBone Hip => Bones[0] as HumanoidPhysBone;
        public HumanoidPhysBone Knee => Bones[1] as HumanoidPhysBone;
        public HumanoidPhysBone Ankle => Bones[2] as HumanoidPhysBone;

        IBone IHumanLeg.Hip => Hip;

        IBone IHumanLeg.Knee => Knee;

        IBone IHumanLeg.Ankle => Ankle;

        private RelativeBone _relativeToe = null;
        public IBone Toe => _relativeToe;

        private IHumanLeg _leg;

        public override void Initiate()
        {
            base.Initiate();

            string prefix = isLeft ? "Left" : "Right";
            _bones[0] = new HumanoidPhysBone($"{prefix} Hip", null, JointAngularLimits.Free);
            _bones[1] = new HumanoidPhysBone($"{prefix} Knee", Hip, JointAngularLimits.Free);
            _bones[2] = new HumanoidPhysBone($"{prefix} Ankle", Knee, JointAngularLimits.Free);
        }

        public override void Solve()
        {
            var hipTarget = Hip.Parent.TransformBone(_leg.Hip.Parent, _leg.Hip);
            Hip.Solve(hipTarget);

            Hip.SetConnectedAnchor(hipTarget.position);

            var kneeTarget = Hip.TransformBone(_leg.Hip, _leg.Knee);
            Knee.Solve(kneeTarget);

            Knee.SetConnectedAnchor(kneeTarget.position);

            var ankleTarget = Knee.TransformBone(_leg.Knee, _leg.Ankle);
            Ankle.Solve(ankleTarget);

            Ankle.SetConnectedAnchor(ankleTarget.position);
        }

        public override void Attach(PhysBoneGroup group)
        {
            FirstBone.Parent = group.FirstBone;
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
            Knee.ConfigurableJoint.ConfigurableJoint.connectedMassScale = 0f;
            Ankle.ConfigurableJoint.ConfigurableJoint.connectedMassScale = 0f;

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
                bottomTransform = SimpleTransform.Create(back() * proportions.kneeOffsetZ + down() * proportions.hipEllipsoid.height, quaternion.identity),

                top = hip,
                topTransform = SimpleTransform.Create(float3.zero, quaternion.identity),
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
                bottomTransform = SimpleTransform.Create(back() * proportions.ankleOffsetZ + down() * proportions.kneeEllipsoid.height, quaternion.identity),

                top = knee,
                topTransform = SimpleTransform.Create(float3.zero, quaternion.identity),
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
                bottomTransform = SimpleTransform.Create(toeBottom, toeRotation),

                top = ankle,
                topTransform = SimpleTransform.Create(float3.zero, quaternion.identity),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }
    }
}
