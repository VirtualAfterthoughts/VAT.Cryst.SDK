using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Proportions;
using VAT.Avatars.Skeletal;
using VAT.Shared.Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VAT.Avatars.Integumentary
{
    using System.Linq;
    using Unity.Mathematics;
    using VAT.Shared.Utilities;

    public partial class HumanoidAvatar : Avatar
    {
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            EditorUpdateEyeCenter();
        }

        public void EditorUpdateEyeCenter()
        {
            if (Initiated)
            {
                float3? eyeCenterRaw = EditorGetEyeCenter();
                if (eyeCenterRaw.HasValue)
                {
                    Skeleton.DataSkeleton.Neck.EyeCenter.position = eyeCenterRaw.Value;
                    Skeleton.DataSkeleton.Neck.EyeCenter.rotation = transform.rotation;
                }
            }
        }

        public float3? EditorGetEyeCenter()
        {
            if (eyeCenterOverride != null)
            {
                return eyeCenterOverride.position;
            }

            if (animator != null)
            {
                Transform leftEye = animator.GetBoneTransform(HumanBodyBones.LeftEye);
                Transform rightEye = animator.GetBoneTransform(HumanBodyBones.RightEye);

                if (leftEye != null && rightEye != null)
                    return (leftEye.position + rightEye.position) * 0.5f;
            }

            return null;
        }

        protected override void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                base.OnDrawGizmos();
                return;
            }
            else if (!Selection.GetTransforms(SelectionMode.Deep).Contains(transform))
            {
                return;
            }

            using (new TempGizmoColor())
            {
                float3? eyeCenter = EditorGetEyeCenter();

                if (eyeCenter.HasValue)
                {
                    Skeleton.DataSkeleton.Neck.EyeCenter.position = eyeCenter.Value;
                    Skeleton.DataSkeleton.Neck.EyeCenter.rotation = transform.rotation;

                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(eyeCenter.Value, 0.02f);
                    Gizmos.color = Color.white;

                    base.OnDrawGizmos();

                    // Draw meshes
                    Gizmos.color = new Color(255f, 0f, 128f, 255f) / 255f;

                    DrawNeckGizmos(Skeleton.DataSkeleton.Neck, Skeleton.PhysSkeleton.Neck, proportions.neckProportions);

                    DrawSpineGizmos(Skeleton.DataSkeleton.Spine, Skeleton.PhysSkeleton.Spine, proportions.spineProportions, proportions.neckProportions);

                    DrawArmGizmos(Skeleton.DataSkeleton.LeftArm, Skeleton.PhysSkeleton.LeftArm, proportions.leftArmProportions);
                    DrawArmGizmos(Skeleton.DataSkeleton.RightArm, Skeleton.PhysSkeleton.RightArm, proportions.rightArmProportions);

                    DrawLegGizmos(Skeleton.DataSkeleton.LeftLeg, Skeleton.PhysSkeleton.LeftLeg, proportions.leftLegProportions);
                    DrawLegGizmos(Skeleton.DataSkeleton.RightLeg, Skeleton.PhysSkeleton.RightLeg, proportions.rightLegProportions);
                }
            }
        }

        private void DrawNeckGizmos(HumanoidNeck neck, HumanoidPhysNeck physNeck, HumanoidNeckProportions proportions)
        {
            Gizmos.DrawWireMesh(physNeck.GenerateSkullMesh(proportions), neck.Skull.position, neck.Skull.rotation);
            Gizmos.DrawWireMesh(physNeck.GenerateUpperNeckMesh(proportions), neck.C1Vertebra.position, neck.C1Vertebra.rotation);
            Gizmos.DrawWireMesh(physNeck.GenerateLowerNeckMesh(proportions), neck.C4Vertebra.position, neck.C4Vertebra.rotation);
        }

        private void DrawSpineGizmos(HumanoidSpine spine, HumanoidPhysSpine physSpine, HumanoidSpineProportions proportions, HumanoidNeckProportions neck)
        {
            Gizmos.DrawWireMesh(physSpine.GenerateUpperChestMesh(proportions, neck), spine.T1Vertebra.position, spine.T1Vertebra.rotation);
            Gizmos.DrawWireMesh(physSpine.GenerateChestMesh(proportions), spine.T7Vertebra.position, spine.T7Vertebra.rotation);
            Gizmos.DrawWireMesh(physSpine.GenerateSpineMesh(proportions), spine.L1Vertebra.position, spine.L1Vertebra.rotation);
            Gizmos.DrawWireMesh(physSpine.GeneratePelvisMesh(proportions), spine.Sacrum.position, spine.Sacrum.rotation);
        }

        private void DrawArmGizmos(HumanoidArm arm, HumanoidPhysArm physArm, HumanoidArmProportions proportions)
        {
            Gizmos.DrawWireMesh(physArm.GenerateClavicleMesh(proportions), arm.Clavicle.position, arm.Clavicle.rotation);
            Gizmos.DrawWireMesh(physArm.GenerateShoulderBladeMesh(proportions), arm.Scapula.position, arm.Scapula.rotation);
            Gizmos.DrawWireMesh(physArm.GenerateUpperArmMesh(proportions), arm.UpperArm.position, arm.UpperArm.rotation);
            Gizmos.DrawWireMesh(physArm.GenerateElbowMesh(proportions), arm.Elbow.position, arm.Elbow.rotation);
            Gizmos.DrawWireMesh(HumanoidPhysHand.GenerateHandMesh(proportions), arm.Hand.Hand.position, arm.Hand.Hand.rotation);
            Gizmos.DrawWireMesh(HumanoidPhysHand.GenerateKnuckleMesh(proportions), arm.Hand.Hand.position + arm.Hand.Hand.forward * proportions.handProportions.wristEllipsoid.height, arm.Hand.Hand.rotation);
        }

        private void DrawLegGizmos(HumanoidLeg leg, HumanoidPhysLeg physLeg, HumanoidLegProportions proportions)
        {
            Gizmos.DrawWireMesh(physLeg.GenerateHipMesh(proportions), leg.Hip.position, leg.Hip.rotation);
            Gizmos.DrawWireMesh(physLeg.GenerateKneeMesh(proportions), leg.Knee.position, leg.Knee.rotation);
            Gizmos.DrawWireMesh(physLeg.GenerateAnkleMesh(proportions), leg.Ankle.position, leg.Ankle.rotation);
        }
#endif
    }
}
