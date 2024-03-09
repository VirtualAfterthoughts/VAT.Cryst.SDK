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

        private IHumanSpine _spine;

        public override void Initiate()
        {
            base.Initiate();

            _bones[0] = new HumanoidPhysBone("Root");
            _bones[1] = new HumanoidPhysBone("Pelvis", Root, HumanoidConstants.PelvisLimits);
            _bones[2] = new HumanoidPhysBone("Spine", Sacrum, HumanoidConstants.SpineLimits);
            _bones[3] = new HumanoidPhysBone("Chest", L1Vertebra, HumanoidConstants.ChestLimits);
            _bones[4] = new HumanoidPhysBone("Upper Chest", T7Vertebra, HumanoidConstants.UpperChestLimits);
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


        private Vector3 errorLast;

        public override void Solve()
        {
            float frequency = 50000f;
            float damping = 1000f;
            Root.ConfigurableJoint.Rigidbody.Rigidbody.inertiaTensor = new Vector3(100f, 100f, 100f);
            // var up = Root.Transform.up;
            // var targetUp = _spine.Root.up;

            // Quaternion targetRotation = Quaternion.FromToRotation(up, targetUp) * Root.transform.rotation;
            var targetRotation = _spine.Root.Transform.rotation;

            var startRotation = Root.Transform.rotation;
            Quaternion desiredRotation = targetRotation;
            var kp = (6f * frequency) * (6f * frequency) * 0.25f;
            var kd = 4.5f * frequency * damping;
            float dt = Time.fixedDeltaTime;
            float g = 1 / (1 + kd * dt + kp * dt * dt);
            float ksg = kp * g;
            float kdg = (kd + kp * dt) * g;
            Quaternion q = desiredRotation * Quaternion.Inverse(startRotation);
            // Q can be the-long-rotation-around-the-sphere eg. 350 degrees
            // We want the equivalant short rotation eg. -10 degrees
            // Check if rotation is greater than 190 degees == q.w is negative
            if (q.w < 0)
            {
                // Convert the quaterion to eqivalent "short way around" quaterion
                q.x = -q.x;
                q.y = -q.y;
                q.z = -q.z;
                q.w = -q.w;
            }
            q.ToAngleAxis(out float xMag, out Vector3 x);
            x.Normalize();
            x *= Mathf.Deg2Rad;
            Vector3 error = x * xMag;
            Vector3 derivative = kdg * ((error - errorLast) / dt);
            errorLast = error;
            Vector3 pidv = ksg * error + derivative;
            Quaternion rotInertia2World = Root.ConfigurableJoint.Rigidbody.Rigidbody.inertiaTensorRotation * startRotation;
            pidv = Quaternion.Inverse(rotInertia2World) * pidv;
            pidv.Scale(Root.ConfigurableJoint.Rigidbody.Rigidbody.inertiaTensor);
            pidv = rotInertia2World * pidv;
            Root.ConfigurableJoint.Rigidbody.Rigidbody.AddTorque(pidv);

            var sacrumTarget = Root.TransformBone(_spine.Root, _spine.Sacrum);
            Sacrum.Solve(sacrumTarget);

            Sacrum.SetConnectedAnchor(sacrumTarget.position);

            var l1Target = Sacrum.TransformBone(_spine.Sacrum, _spine.L1Vertebra);
            L1Vertebra.Solve(l1Target);

            L1Vertebra.SetConnectedAnchor(l1Target.position);

            var t7Target = L1Vertebra.TransformBone(_spine.L1Vertebra, _spine.T7Vertebra);
            T7Vertebra.Solve(t7Target);

            T7Vertebra.SetConnectedAnchor(t7Target.position);

            var t1Target = T7Vertebra.TransformBone(_spine.T7Vertebra, _spine.T1Vertebra);
            T1Vertebra.Solve(t1Target);

            T1Vertebra.SetConnectedAnchor(t1Target.position);
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
                bottomTransform = SimpleTransform.Create(float3.zero, quaternion.identity),

                top = lowerNeck,
                topTransform = SimpleTransform.Create(forward() * -proportions.upperChestOffsetZ + up() * neck.lowerNeckEllipsoid.height, quaternion.identity),
            };

            // Create upper chest -> chest
            EllipseCylinderMesh breast = new()
            {
                bottom = chest,
                bottomTransform = SimpleTransform.Create(forward() * proportions.chestOffsetZ + down() * proportions.upperChestEllipsoid.height, quaternion.identity),

                top = upperChest,
                topTransform = SimpleTransform.Create(float3.zero, quaternion.identity),
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
                bottomTransform = SimpleTransform.Create(forward() * proportions.spineOffsetZ + down() * proportions.chestEllipsoid.height, quaternion.identity),

                top = chest,
                topTransform = SimpleTransform.Create(float3.zero, quaternion.identity),
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
                bottomTransform = SimpleTransform.Create(forward() * proportions.pelvisOffsetZ + down() * proportions.spineEllipsoid.height, quaternion.identity),

                top = spine,
                topTransform = SimpleTransform.Create(float3.zero, quaternion.identity),
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
                bottomTransform = SimpleTransform.Create(down() * proportions.pelvisEllipsoid.height, quaternion.identity),

                top = pelvis,
                topTransform = SimpleTransform.Create(float3.zero, quaternion.identity),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }
    }
}
