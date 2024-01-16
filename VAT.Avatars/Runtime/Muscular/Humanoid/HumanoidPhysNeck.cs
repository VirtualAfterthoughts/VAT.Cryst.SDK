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

    public class HumanoidPhysNeck : HumanoidPhysBoneGroup, IPoseableT<HumanoidNeck>
    {
        public override int BoneCount => 3;

        public HumanoidPhysBone C4Vertebra => Bones[0];
        public HumanoidPhysBone C1Vertebra => Bones[1];
        public HumanoidPhysBone Skull => Bones[2];

        private HumanoidNeck _neck;

        public override void Initiate() {
            base.Initiate();

            _bones[0] = new HumanoidPhysBone("Lower Neck", null, HumanoidConstants.LowerNeckLimits);
            _bones[1] = new HumanoidPhysBone("Upper Neck", C4Vertebra, HumanoidConstants.UpperNeckLimits);
            _bones[2] = new HumanoidPhysBone("Skull", C1Vertebra, HumanoidConstants.SkullLimits);
        }

        public void WriteProportions(HumanoidNeckProportions proportions) { 
            C4Vertebra.SetMesh(GenerateLowerNeckMesh(proportions));
            C1Vertebra.SetMesh(GenerateUpperNeckMesh(proportions));
            Skull.SetMesh(GenerateSkullMesh(proportions));
        }

        public SimpleTransform GetHead() {
            return Skull.Transform;
        }

        public void MatchPose(HumanoidNeck neck) {
            _neck = neck;

            C4Vertebra.MatchBone(neck.C4Vertebra);
            C1Vertebra.MatchBone(neck.C1Vertebra);
            Skull.MatchBone(neck.Skull);
        }

        public override void Solve()
        {
            var c4VertebraTarget = C4Vertebra.Parent.TransformBone(_neck.C4Vertebra.GetChild(0), _neck.C4Vertebra);
            C4Vertebra.Solve(c4VertebraTarget);

            C4Vertebra.SetConnectedAnchor(c4VertebraTarget.position);

            var c1VertebraTarget = C4Vertebra.TransformBone(_neck.C4Vertebra, _neck.C1Vertebra);
            C1Vertebra.Solve(c1VertebraTarget);

            C1Vertebra.SetConnectedAnchor(c1VertebraTarget.position);

            var skullTarget = C1Vertebra.TransformBone(_neck.C1Vertebra, _neck.Skull);
            Skull.Solve(skullTarget);

            Skull.SetConnectedAnchor(skullTarget.position);
        }

        public Mesh GenerateSkullMesh(HumanoidNeckProportions proportions) {
            // Convert ellipsoids to ellipses
            var skull = proportions.skullEllipsoid.AsInterface().Convert<Ellipse>();

            // Create forehead -> top of head
            EllipseCylinderMesh top = new()
            {
                bottom = proportions.foreheadEllipse,
                bottomTransform = SimpleTransform.Create(up() * proportions.skullEllipsoid.height * 0.3f, quaternion.identity),

                top = proportions.topEllipse,
                topTransform = SimpleTransform.Create(up() * proportions.skullEllipsoid.height * 0.5f, quaternion.identity),
            };

            // Create skull -> forehead
            EllipseCylinderMesh forehead = new()
            {
                bottom = skull,
                bottomTransform = SimpleTransform.Create(float3.zero, quaternion.identity),

                top = proportions.foreheadEllipse,
                topTransform = SimpleTransform.Create(up() * proportions.skullEllipsoid.height * 0.3f, quaternion.identity),
            };

            // Create skull -> jaw
            EllipseCylinderMesh jaw = new() {
                bottom = proportions.jawEllipse,
                bottomTransform = SimpleTransform.Create(down() * proportions.skullEllipsoid.height * 0.3f, Quaternion.AngleAxis(25f, right())),

                top = skull,
                topTransform = SimpleTransform.Create(float3.zero, quaternion.identity),
            };

            // Combine ellipses and create mesh
            return MeshDescriptor.Combine(top.CreateDescriptor(), MeshDescriptor.Combine(forehead.CreateDescriptor(), jaw.CreateDescriptor())).CreateMesh();
        }

        public Mesh GenerateUpperNeckMesh(HumanoidNeckProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var upperNeck = proportions.upperNeckEllipsoid.AsInterface().Convert<Ellipse>();
            var lowerNeck = proportions.lowerNeckEllipsoid.AsInterface().Convert<Ellipse>();

            // Create larger upper neck -> upper neck
            EllipseCylinderMesh jawToNeck = new()
            {
                bottom = upperNeck,
                bottomTransform = SimpleTransform.Create(float3.zero, quaternion.identity),

                top = upperNeck.Scaled(new float2(1f, 1.2f)),
                topTransform = SimpleTransform.Create(up() * proportions.upperNeckEllipsoid.height * 0.7f, quaternion.identity),
            };

            // Create upper neck -> lower neck
            EllipseCylinderMesh upperToLower = new()
            {
                bottom = lowerNeck,
                bottomTransform = SimpleTransform.Create(back() * proportions.lowerNeckOffsetZ + down() * proportions.upperNeckEllipsoid.height * 0.5f, quaternion.identity),

                top = upperNeck,
                topTransform = SimpleTransform.Create(float3.zero, quaternion.identity),
            };

            // Combine ellipses and create mesh
            return MeshDescriptor.Combine(jawToNeck.CreateDescriptor(), upperToLower.CreateDescriptor()).CreateMesh();
        }

        public Mesh GenerateLowerNeckMesh(HumanoidNeckProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var lowerNeck = proportions.lowerNeckEllipsoid.AsInterface().Convert<Ellipse>();

            // Create upper neck -> lower neck
            EllipseCylinderMesh upperToLower = new()
            {
                bottom = lowerNeck,
                bottomTransform = SimpleTransform.Create(float3.zero, quaternion.identity),

                top = lowerNeck,
                topTransform = SimpleTransform.Create(up() * proportions.upperNeckEllipsoid.height * 0.5f, quaternion.identity),
            };

            // Create lower neck -> larger lower neck
            EllipseCylinderMesh lowerToChest = new()
            {
                bottom = lowerNeck.Scaled(new float2(1f, 1.2f)),
                bottomTransform = SimpleTransform.Create(down() * proportions.lowerNeckEllipsoid.height * 0.5f, quaternion.identity),

                top = lowerNeck,
                topTransform = SimpleTransform.Create(float3.zero, quaternion.identity),
            };

            // Combine ellipses and create mesh
            return MeshDescriptor.Combine(lowerToChest.CreateDescriptor(), upperToLower.CreateDescriptor()).CreateMesh();
        }
    }
}