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

    using VAT.Avatars.Bones;
    using VAT.Shared.Extensions;

    public class HumanoidPhysSpine : HumanoidPhysBoneGroup, IHumanSpine
    {
        public override int BoneCount => 5;

        public HumanoidPhysBone Root => Bones[0] as HumanoidPhysBone;
        public HumanoidPhysBone Sacrum => Bones[1] as HumanoidPhysBone;
        public HumanoidPhysBone L1Vertebra => Bones[2] as HumanoidPhysBone;
        public HumanoidPhysBone T7Vertebra => Bones[3] as HumanoidPhysBone;
        public HumanoidPhysBone T1Vertebra => Bones[4] as HumanoidPhysBone;

        public override PhysBone FirstBone => Sacrum;

        public override bool PivotAtParent => true;

        IBone IHumanSpine.Sacrum => Sacrum;

        IBone IHumanSpine.L1Vertebra => L1Vertebra;

        IBone IHumanSpine.T7Vertebra => T7Vertebra;

        IBone IHumanSpine.T1Vertebra => T1Vertebra;

        IBone IHumanSpine.Root => Root;

        IBone IHumanSpine.TargetRoot => Root;

        private IHumanSpine _spine;

        public override void Initiate()
        {
            base.Initiate();

            _bones[0] = new HumanoidPhysBone("Root");
            _bones[1] = new HumanoidPhysBone("Pelvis", Root, HumanoidConstants.PelvisLimits);
            _bones[2] = new HumanoidPhysBone("Spine", Sacrum, HumanoidConstants.SpineLimits);
            _bones[3] = new HumanoidPhysBone("Chest", L1Vertebra, HumanoidConstants.ChestLimits);
            _bones[4] = new HumanoidPhysBone("Upper Chest", T7Vertebra, HumanoidConstants.UpperChestLimits);

            Root.ConfigureJoint(500000000f);

            Sacrum.ConfigurableJoint.ConfigurableJoint.SetMotion(ConfigurableJointMotion.Free, ConfigurableJointMotion.Free);
            Root.ConfigurableJoint.ConfigurableJoint.SetMotion(ConfigurableJointMotion.Locked, ConfigurableJointMotion.Free);

            // Joint limits
            var l1Joint = L1Vertebra.ConfigurableJoint.ConfigurableJoint;
            l1Joint.angularXMotion = l1Joint.angularYMotion = l1Joint.angularZMotion = ConfigurableJointMotion.Limited;
            l1Joint.angularZLimit = l1Joint.angularYLimit = l1Joint.highAngularXLimit = new SoftJointLimit() { limit = 20f };
            l1Joint.lowAngularXLimit = new SoftJointLimit() { limit = -20f };

            var t7Joint = T7Vertebra.ConfigurableJoint.ConfigurableJoint;
            t7Joint.angularXMotion = t7Joint.angularYMotion = t7Joint.angularZMotion = ConfigurableJointMotion.Limited;
            t7Joint.angularZLimit = t7Joint.angularYLimit = t7Joint.highAngularXLimit = new SoftJointLimit() { limit = 20f };
            t7Joint.lowAngularXLimit = new SoftJointLimit() { limit = -20f };

            var t1Joint = T1Vertebra.ConfigurableJoint.ConfigurableJoint;
            t1Joint.angularXMotion = t1Joint.angularYMotion = t1Joint.angularZMotion = ConfigurableJointMotion.Limited;
            t1Joint.angularZLimit = t1Joint.angularYLimit = t1Joint.highAngularXLimit = new SoftJointLimit() { limit = 20f };
            t1Joint.lowAngularXLimit = new SoftJointLimit() { limit = -20f };
        }

        public void WriteProportions(HumanoidSpineProportions proportions, HumanoidNeckProportions neck)
        {
            Sacrum.SetMesh(GeneratePelvisMesh(proportions));
            L1Vertebra.SetMesh(GenerateSpineMesh(proportions));
            T7Vertebra.SetMesh(GenerateChestMesh(proportions));
            T1Vertebra.SetMesh(GenerateUpperChestMesh(proportions, neck));
        }

        public void MatchPose(IHumanSpine spine)
        {
            _spine = spine;

            Sacrum.MatchBone(spine.Sacrum);
            L1Vertebra.MatchBone(spine.L1Vertebra);
            T7Vertebra.MatchBone(spine.T7Vertebra);
            T1Vertebra.MatchBone(spine.T1Vertebra);
        }


        public override void Solve()
        {
            Root.Solve(_spine.TargetRoot.Transform);

            var sacrumTarget = Root.TransformBone(_spine.Root, _spine.Sacrum);
            Sacrum.Solve(sacrumTarget);

            Sacrum.SetConnectedAnchor(sacrumTarget.Position);

            var l1Target = Sacrum.TransformBone(_spine.Sacrum, _spine.L1Vertebra);
            L1Vertebra.Solve(l1Target);

            L1Vertebra.ConfigurableJoint.ConfigurableJoint.connectedAnchor = Vector3.zero;
            L1Vertebra.ConfigurableJoint.ConfigurableJoint.anchor = _spine.L1Vertebra.Transform.InverseTransformPoint(_spine.Sacrum.Transform.Position);

            var t7Target = L1Vertebra.TransformBone(_spine.L1Vertebra, _spine.T7Vertebra);
            T7Vertebra.Solve(t7Target);

            T7Vertebra.ConfigurableJoint.ConfigurableJoint.connectedAnchor = Vector3.zero;
            T7Vertebra.ConfigurableJoint.ConfigurableJoint.anchor = _spine.T7Vertebra.Transform.InverseTransformPoint(_spine.L1Vertebra.Transform.Position);

            var t1Target = T7Vertebra.TransformBone(_spine.T7Vertebra, _spine.T1Vertebra);
            T1Vertebra.Solve(t1Target);

            T1Vertebra.ConfigurableJoint.ConfigurableJoint.connectedAnchor = Vector3.zero;
            T1Vertebra.ConfigurableJoint.ConfigurableJoint.anchor = _spine.T1Vertebra.Transform.InverseTransformPoint(_spine.T7Vertebra.Transform.Position);
        }

        public Mesh GenerateUpperChestMesh(HumanoidSpineProportions proportions, HumanoidNeckProportions neck)
        {
            // Convert ellipsoids to ellipses
            var lowerNeck = proportions.upperChestEllipsoid.Convert<Ellipse>().Scaled(new float2(0.9f, 0.6f));
            var upperChest = proportions.upperChestEllipsoid.Convert<Ellipse>();
            var chest = proportions.chestEllipsoid.Convert<Ellipse>();

            // Create lower neck -> upper chest
            EllipseCylinderMesh collar = new()
            {
                bottom = upperChest,
                bottomTransform = new SimpleTransform(float3.zero, quaternion.identity),

                top = lowerNeck,
                topTransform = new SimpleTransform(forward() * -proportions.upperChestOffsetZ + up() * neck.lowerNeckEllipsoid.height, quaternion.identity),
            };

            // Create upper chest -> chest
            EllipseCylinderMesh breast = new()
            {
                bottom = chest,
                bottomTransform = new SimpleTransform(forward() * proportions.chestOffsetZ + down() * proportions.upperChestEllipsoid.height, quaternion.identity),

                top = upperChest,
                topTransform = new SimpleTransform(float3.zero, quaternion.identity),
            };

            // Create mesh
            return MeshDescriptor.Combine(collar.CreateDescriptor(), breast.CreateDescriptor()).CreateMesh();
        }

        public Mesh GenerateChestMesh(HumanoidSpineProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var chest = proportions.chestEllipsoid.Convert<Ellipse>();
            var spine = proportions.spineEllipsoid.Convert<Ellipse>();

            // Create chest -> spine
            EllipseCylinderMesh cylinder = new()
            {
                bottom = spine,
                bottomTransform = new SimpleTransform(forward() * proportions.spineOffsetZ + down() * proportions.chestEllipsoid.height, quaternion.identity),

                top = chest,
                topTransform = new SimpleTransform(float3.zero, quaternion.identity),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }

        public Mesh GenerateSpineMesh(HumanoidSpineProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var spine = proportions.spineEllipsoid.Convert<Ellipse>();
            var pelvis = proportions.pelvisEllipsoid.Convert<Ellipse>();

            // Create spine -> pelvis
            EllipseCylinderMesh cylinder = new()
            {
                bottom = pelvis,
                bottomTransform = new SimpleTransform(forward() * proportions.pelvisOffsetZ + down() * proportions.spineEllipsoid.height, quaternion.identity),

                top = spine,
                topTransform = new SimpleTransform(float3.zero, quaternion.identity),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }

        public Mesh GeneratePelvisMesh(HumanoidSpineProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var pelvis = proportions.pelvisEllipsoid.Convert<Ellipse>();
            var groin = pelvis.Scaled(0.6f);

            // Create pelvis -> groin
            EllipseCylinderMesh cylinder = new()
            {
                bottom = groin,
                bottomTransform = new SimpleTransform(down() * proportions.pelvisEllipsoid.height, quaternion.identity),

                top = pelvis,
                topTransform = new SimpleTransform(float3.zero, quaternion.identity),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }
    }
}
