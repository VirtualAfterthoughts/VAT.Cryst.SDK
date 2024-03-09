using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static Unity.Mathematics.math;

using VAT.Avatars.Proportions;
using VAT.Avatars.Constants;
using VAT.Avatars.Skeletal;

using VAT.Cryst.Interfaces;

using VAT.Shared.Data;

namespace VAT.Avatars.Muscular
{
    using Unity.Mathematics;
    using UnityEngine.XR;
    using VAT.Avatars.REWORK;
    using VAT.Cryst.Delegates;

    public class HumanoidPhysArm : HumanoidPhysBoneGroup, IPoseableT<IHumanArm>, IHumanArm
    {
        public bool isLeft = false;
        public override int BoneCount => 4;

        public override int SubGroupCount => 1;

        public HumanoidPhysBone Clavicle => TBones[0];
        public HumanoidPhysBone Scapula => TBones[1];
        public HumanoidPhysBone UpperArm => TBones[2];
        public HumanoidPhysBone Elbow => TBones[3];

        public HumanoidPhysHand Hand => SubGroups[0] as HumanoidPhysHand;

        IBone IHumanArm.Clavicle => Clavicle;

        IBone IHumanArm.Scapula => Scapula;

        IBone IArmGroup.UpperArm => UpperArm;

        IBone IArmGroup.Elbow => Elbow;

        private RelativeBone _relativeWrist = null;
        IBone IHumanArm.Wrist => _relativeWrist;

        IHandGroup IArmGroup.Hand => Hand;

        private RelativeBone _relativeCarpal = null;
        public IBone Carpal => _relativeCarpal;

        public SimpleTransform Target => _arm.Target;

        private IHumanArm _arm;

        public event TargetProcessorCallback OnProcessTarget
        {
            add
            {
                _arm.OnProcessTarget += value;
            }
            remove
            {
                _arm.OnProcessTarget -= value;
            }
        }

        public override void Initiate() {
            base.Initiate();

            string prefix = isLeft ? "Left" : "Right";
            _bones[0] = new HumanoidPhysBone($"{prefix} Clavicle", null, HumanoidConstants.ClavicleLimits);
            _bones[1] = new HumanoidPhysBone($"{prefix} Shoulder Blade", Clavicle, HumanoidConstants.ScapulaLimits);
            _bones[2] = new HumanoidPhysBone($"{prefix} Upper Arm", Scapula, HumanoidConstants.UpperArmLimits);
            _bones[3] = new HumanoidPhysBone($"{prefix} Elbow", UpperArm, HumanoidConstants.ElbowLimits);

            _subGroups[0] = new HumanoidPhysHand(this);

            Elbow.ConfigurableJoint.ConfigurableJoint.axis = Vector3.down * (isLeft ? -1f : 1f);
            Elbow.ConfigurableJoint.ConfigurableJoint.secondaryAxis = Vector3.forward;
        }

        public override void Solve()
        {
            Clavicle.Solve(Clavicle.Parent.TransformBone(_arm.Clavicle.Parent, _arm.Clavicle));
            Scapula.Solve(Scapula.Parent.TransformBone(_arm.Scapula.Parent, _arm.Scapula));
            UpperArm.Solve(UpperArm.Parent.TransformBone(_arm.UpperArm.Parent, _arm.UpperArm));
            Elbow.Solve(Elbow.Parent.TransformBone(_arm.Elbow.Parent, _arm.Elbow));
            Hand.Hand.Solve(Elbow.TransformBone(_arm.Elbow, _arm.Hand.Hand));
        }

        public void WriteProportions(HumanoidArmProportions proportions)
        {
            Clavicle.SetMesh(GenerateClavicleMesh(proportions));
            Scapula.SetMesh(GenerateShoulderBladeMesh(proportions));
            UpperArm.SetMesh(GenerateUpperArmMesh(proportions));
            Elbow.SetMesh(GenerateElbowMesh(proportions));
            Hand.Hand.SetMesh(GenerateHandMesh(proportions));
        }

        public void MatchPose(IHumanArm arm)
        {
            _arm = arm;

            Clavicle.MatchBone(arm.Clavicle);
            Scapula.MatchBone(arm.Scapula);
            UpperArm.MatchBone(arm.UpperArm);
            Elbow.MatchBone(arm.Elbow);
            Hand.MatchPose(arm.Hand);

            _relativeWrist = new RelativeBone(Hand.Hand, arm.Hand.Hand, arm.Wrist);
            _relativeCarpal = new RelativeBone(Hand.Hand, arm.Hand.Hand, arm.Carpal);
        }

        public Mesh GenerateClavicleMesh(HumanoidArmProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var clavicle = proportions.clavicleEllipsoid.Convert<Ellipse>();

            // Create clavicle top to bottom
            float3 offset = right() * (isLeft ? -1f : 1f) * clavicle.radius.x;

            EllipseCylinderMesh cylinder = new EllipseCylinderMesh()
            {
                bottom = clavicle,
                bottomTransform = SimpleTransform.Create(offset + down() * proportions.clavicleEllipsoid.height, quaternion.identity),

                top = clavicle,
                topTransform = SimpleTransform.Create(offset + up() * proportions.clavicleEllipsoid.height, quaternion.identity),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }

        public Mesh GenerateShoulderBladeMesh(HumanoidArmProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var scapula = proportions.shoulderBladeEllipsoid.Convert<Ellipse>();

            // Create shoulder blade top to bottom
            EllipseCylinderMesh cylinder = new EllipseCylinderMesh()
            {
                bottom = scapula,
                bottomTransform = SimpleTransform.Create(down() * proportions.shoulderBladeEllipsoid.height, quaternion.identity),

                top = scapula,
                topTransform = SimpleTransform.Create(up() * proportions.shoulderBladeEllipsoid.height, quaternion.identity),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }

        public Mesh GenerateUpperArmMesh(HumanoidArmProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var upperArm = proportions.upperArmEllipsoid.Convert<Ellipse>();
            var elbow = proportions.elbowEllipsoid.Convert<Ellipse>();

            // Create upperArm -> elbow
            quaternion rotation = Quaternion.AngleAxis(90f, right());

            EllipseCylinderMesh cylinder = new EllipseCylinderMesh()
            {
                bottom = upperArm,
                bottomTransform = SimpleTransform.Create(float3.zero, rotation),

                top = elbow,
                topTransform = SimpleTransform.Create(forward() * proportions.upperArmEllipsoid.height, rotation),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }

        public Mesh GenerateElbowMesh(HumanoidArmProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var elbow = proportions.elbowEllipsoid.Convert<Ellipse>();
            var wrist = proportions.handProportions.wristEllipsoid.Convert<Ellipse>();

            // Create elbow -> wrist
            quaternion rotation = Quaternion.AngleAxis(90f, right());

            EllipseCylinderMesh cylinder = new EllipseCylinderMesh()
            {
                bottom = elbow,
                bottomTransform = SimpleTransform.Create(float3.zero, rotation),

                top = wrist,
                topTransform = SimpleTransform.Create(forward() * proportions.elbowEllipsoid.height, rotation),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }

        public Mesh GenerateHandMesh(HumanoidArmProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var wrist = proportions.handProportions.wristEllipsoid.Convert<Ellipse>();
            var knuckle = proportions.handProportions.knuckleEllipsoid.Convert<Ellipse>();

            // Create wrist -> knuckle
            quaternion rotation = Quaternion.AngleAxis(90f, right());

            EllipseCylinderMesh cylinder = new EllipseCylinderMesh()
            {
                bottom = wrist,
                bottomTransform = SimpleTransform.Create(float3.zero, rotation),

                top = knuckle,
                topTransform = SimpleTransform.Create(forward() * proportions.handProportions.wristEllipsoid.height, rotation),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }

        public Mesh GenerateKnuckleMesh(HumanoidArmProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var knuckle = proportions.handProportions.knuckleEllipsoid.Convert<Ellipse>();

            // Create knuckle -> finger top
            quaternion rotation = Quaternion.AngleAxis(90f, right());

            EllipseCylinderMesh cylinder = new()
            {
                bottom = knuckle,
                bottomTransform = SimpleTransform.Create(float3.zero, rotation),

                top = knuckle,
                topTransform = SimpleTransform.Create(forward() * proportions.handProportions.knuckleEllipsoid.height, rotation),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }
    }
}
