using UnityEngine;

using VAT.Avatars.Proportions;
using VAT.Avatars.Constants;

using VAT.Shared.Data;

using static Unity.Mathematics.math;

namespace VAT.Avatars.Muscular
{
    using Unity.Mathematics;
    using VAT.Avatars.Bones;

    public class HumanoidPhysNeck : HumanoidPhysBoneGroup, IHumanNeck
    {
        public override int BoneCount => 3;

        public HumanoidPhysBone C4Vertebra => Bones[0] as HumanoidPhysBone;
        public HumanoidPhysBone C1Vertebra => Bones[1] as HumanoidPhysBone;
        public HumanoidPhysBone Skull => Bones[2] as HumanoidPhysBone;

        IBone IHumanNeck.C4Vertebra => C4Vertebra;

        IBone IHumanNeck.C1Vertebra => C1Vertebra;

        IBone IHumanNeck.Skull => Skull;

        IBone IHumanNeck.EyeCenter => _relativeEyeCenter;

        private IHumanNeck _neck;

        private RelativeBone _relativeEyeCenter;

        public override void Initiate()
        {
            base.Initiate();

            _bones[0] = new HumanoidPhysBone("Lower Neck", null, HumanoidConstants.LowerNeckLimits);
            _bones[1] = new HumanoidPhysBone("Upper Neck", C4Vertebra, HumanoidConstants.UpperNeckLimits);
            _bones[2] = new HumanoidPhysBone("Skull", C1Vertebra, HumanoidConstants.SkullLimits);
        }

        public void WriteProportions(HumanoidNeckProportions proportions)
        {
            C4Vertebra.SetMesh(GenerateLowerNeckMesh(proportions));
            C1Vertebra.SetMesh(GenerateUpperNeckMesh(proportions));
            Skull.SetMesh(GenerateSkullMesh(proportions));
        }

        public SimpleTransform GetEyeCenter()
        {
            return Skull.TransformBone(_neck.Skull, _neck.EyeCenter);
        }

        public void MatchPose(IHumanNeck neck)
        {
            _neck = neck;

            C4Vertebra.MatchBone(neck.C4Vertebra);
            C1Vertebra.MatchBone(neck.C1Vertebra);
            Skull.MatchBone(neck.Skull);

            _relativeEyeCenter = new RelativeBone(Skull, neck.Skull, neck.EyeCenter);
        }

        public override void Solve()
        {
            var c4VertebraTarget = C4Vertebra.Parent.TransformBone(_neck.C4Vertebra.GetChild(0), _neck.C4Vertebra);
            C4Vertebra.Solve(c4VertebraTarget);

            C4Vertebra.ConfigurableJoint.ConfigurableJoint.connectedAnchor = Vector3.zero;
            C4Vertebra.ConfigurableJoint.ConfigurableJoint.anchor = _neck.C4Vertebra.Transform.InverseTransformPoint(_neck.C4Vertebra.GetChild(0).Transform.Position);

            var c1VertebraTarget = C4Vertebra.TransformBone(_neck.C4Vertebra, _neck.C1Vertebra);
            C1Vertebra.Solve(c1VertebraTarget);

            C1Vertebra.ConfigurableJoint.ConfigurableJoint.connectedAnchor = Vector3.zero;
            C1Vertebra.ConfigurableJoint.ConfigurableJoint.anchor = _neck.C1Vertebra.Transform.InverseTransformPoint(_neck.C4Vertebra.Transform.Position);

            var skullTarget = C1Vertebra.TransformBone(_neck.C1Vertebra, _neck.Skull);
            Skull.Solve(skullTarget);

            Skull.ConfigurableJoint.ConfigurableJoint.connectedAnchor = Vector3.zero;
            Skull.ConfigurableJoint.ConfigurableJoint.anchor = _neck.Skull.Transform.InverseTransformPoint(_neck.C1Vertebra.Transform.Position);
        }

        public Mesh GenerateSkullMesh(HumanoidNeckProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var skull = proportions.skullEllipsoid.Convert<Ellipse>();

            // Create forehead -> top of head
            EllipseCylinderMesh top = new()
            {
                bottom = proportions.foreheadEllipse,
                bottomTransform = new SimpleTransform(up() * proportions.skullEllipsoid.height * 0.3f, quaternion.identity),

                top = proportions.topEllipse,
                topTransform = new SimpleTransform(up() * proportions.skullEllipsoid.height * 0.5f, quaternion.identity),
            };

            // Create skull -> forehead
            EllipseCylinderMesh forehead = new()
            {
                bottom = skull,
                bottomTransform = new SimpleTransform(float3.zero, quaternion.identity),

                top = proportions.foreheadEllipse,
                topTransform = new SimpleTransform(up() * proportions.skullEllipsoid.height * 0.3f, quaternion.identity),
            };

            // Create skull -> jaw
            EllipseCylinderMesh jaw = new()
            {
                bottom = proportions.jawEllipse,
                bottomTransform = new SimpleTransform(down() * proportions.skullEllipsoid.height * 0.3f, Quaternion.AngleAxis(25f, right())),

                top = skull,
                topTransform = new SimpleTransform(float3.zero, quaternion.identity),
            };

            // Combine ellipses and create mesh
            return MeshDescriptor.Combine(top.CreateDescriptor(), MeshDescriptor.Combine(forehead.CreateDescriptor(), jaw.CreateDescriptor())).CreateMesh();
        }

        public Mesh GenerateUpperNeckMesh(HumanoidNeckProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var upperNeck = proportions.upperNeckEllipsoid.Convert<Ellipse>();
            var lowerNeck = proportions.lowerNeckEllipsoid.Convert<Ellipse>();

            // Create larger upper neck -> upper neck
            EllipseCylinderMesh jawToNeck = new()
            {
                bottom = upperNeck,
                bottomTransform = new SimpleTransform(float3.zero, quaternion.identity),

                top = upperNeck.Scaled(new float2(1f, 1.2f)),
                topTransform = new SimpleTransform(up() * proportions.upperNeckEllipsoid.height * 0.7f, quaternion.identity),
            };

            // Create upper neck -> lower neck
            EllipseCylinderMesh upperToLower = new()
            {
                bottom = lowerNeck,
                bottomTransform = new SimpleTransform(forward() * proportions.lowerNeckOffsetZ + down() * proportions.upperNeckEllipsoid.height * 0.5f, quaternion.identity),

                top = upperNeck,
                topTransform = new SimpleTransform(float3.zero, quaternion.identity),
            };

            // Combine ellipses and create mesh
            return MeshDescriptor.Combine(jawToNeck.CreateDescriptor(), upperToLower.CreateDescriptor()).CreateMesh();
        }

        public Mesh GenerateLowerNeckMesh(HumanoidNeckProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var lowerNeck = proportions.lowerNeckEllipsoid.Convert<Ellipse>();

            // Create upper neck -> lower neck
            EllipseCylinderMesh upperToLower = new()
            {
                bottom = lowerNeck,
                bottomTransform = new SimpleTransform(float3.zero, quaternion.identity),

                top = lowerNeck,
                topTransform = new SimpleTransform(up() * proportions.upperNeckEllipsoid.height * 0.5f, quaternion.identity),
            };

            // Create lower neck -> larger lower neck
            EllipseCylinderMesh lowerToChest = new()
            {
                bottom = lowerNeck.Scaled(new float2(1f, 1.2f)),
                bottomTransform = new SimpleTransform(down() * proportions.lowerNeckEllipsoid.height * 0.5f, quaternion.identity),

                top = lowerNeck,
                topTransform = new SimpleTransform(float3.zero, quaternion.identity),
            };

            // Combine ellipses and create mesh
            return MeshDescriptor.Combine(lowerToChest.CreateDescriptor(), upperToLower.CreateDescriptor()).CreateMesh();
        }
    }
}
