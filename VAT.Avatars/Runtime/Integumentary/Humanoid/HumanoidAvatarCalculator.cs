using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using VAT.Avatars.Art;
using VAT.Avatars.Helpers;
using VAT.Avatars.Muscular;
using VAT.Avatars.Proportions;
using VAT.Avatars.Skeletal;
using VAT.Avatars.Vitals;
using VAT.Shared.Data;
using static Unity.Mathematics.math;

namespace VAT.Avatars.Integumentary
{
    public partial class HumanoidAvatar : AvatarT<HumanoidAvatarAnatomy>
    {
#if UNITY_EDITOR
        private void Reset() {
            if (TryGetComponent(out animator)) {
                EditorSetAnimatorPose();
                EditorAutoFillBoneTransforms();
                EditorCalculateProportions();
            }
        }

        [ContextMenu("Set Animator Pose")]
        public void EditorSetAnimatorPose()
        {
            Undo.RegisterChildrenOrderUndo(animator.gameObject, "Set Animator Pose");

            var controller = animator.runtimeAnimatorController;
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Controllers/controller_AvatarDefault");
            animator.Update(0f);
            animator.runtimeAnimatorController = controller;
        }

        [ContextMenu("Calculate Proportions from Animator")]
        public void EditorCalculateProportions()
        {
            Undo.RecordObject(this, "Calculate Proportions from Animator");

            quaternion worldToLocal = inverse(transform.rotation);

            float3? eyeCenterRaw = EditorGetEyeCenter();
            if (!eyeCenterRaw.HasValue) {
                Debug.LogWarning($"Avatar {name} is missing an eye center! Please add an Eye Center Override before calculating proportions!", this);
                return;
            }

            float3 eyeCenter = mul(worldToLocal, eyeCenterRaw.Value);

            Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
            Transform neck = animator.GetBoneTransform(HumanBodyBones.Neck);

            float3 headPosition = mul(worldToLocal, head.position);

            float headDistance = Mathf.Abs(eyeCenter.z - headPosition.z);
            float skullLength = headDistance * 0.8f;

            float skullRadiusX = skullLength * 0.8f;
            float skullRadiusY = skullLength * 0.97f;

            proportions.neckProportions.topEllipse.radius.y = skullRadiusY * 0.441096722f;
            proportions.neckProportions.topEllipse.radius.x = skullRadiusX * 0.322024833f;

            proportions.neckProportions.foreheadEllipse.radius.y = skullRadiusY * 0.975803214f;
            proportions.neckProportions.foreheadEllipse.radius.x = skullRadiusX * 0.888552017f;

            proportions.neckProportions.skullEllipsoid.radius.y = skullRadiusY;
            proportions.neckProportions.skullEllipsoid.radius.x = skullRadiusX;
            proportions.neckProportions.skullZOffset = -skullRadiusX;
            proportions.neckProportions.skullEllipsoid.height = skullLength * 2.32587718f;

            proportions.neckProportions.skullYOffset = Mathf.Abs(eyeCenter.y - headPosition.y) * -0.174878525f;

            proportions.neckProportions.jawEllipse.radius.y = skullRadiusY * 0.967841305f;
            proportions.neckProportions.jawEllipse.radius.x = skullRadiusX * 0.615898028f;

            proportions.neckProportions.upperNeckOffsetZ = headDistance * 0.09f;

            EditorRefreshAvatar();

            float3 dataNeckPosition = mul(worldToLocal, GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.Neck.C1Vertebra.position);
            float3 neckPosition = mul(worldToLocal, neck.position);

            float3 skullPosition = mul(worldToLocal, GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.Neck.Skull.position);
            float skullDifference = 1f - Mathf.Clamp01((headPosition.z - neckPosition.z) / (skullPosition.z - dataNeckPosition.z));

            float upperOffset = (neckPosition.z - dataNeckPosition.z) * skullDifference;
            proportions.neckProportions.upperNeckOffsetZ += upperOffset;

            EditorRefreshAvatar();

            float neckDistance = Mathf.Abs(headPosition.z - neckPosition.z);
            proportions.neckProportions.lowerNeckOffsetZ = neckDistance * 0.7f;

            float neckHeight = Mathf.Abs(dataNeckPosition.y - neckPosition.y);

            proportions.neckProportions.upperNeckEllipsoid.radius.x = skullRadiusX * 0.735778866f;
            proportions.neckProportions.upperNeckEllipsoid.radius.y = skullRadiusY * 0.533928842f;
            proportions.neckProportions.upperNeckEllipsoid.height = neckHeight;

            proportions.neckProportions.lowerNeckEllipsoid = proportions.neckProportions.upperNeckEllipsoid;

            EditorRefreshAvatar();

            Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            float3 hipPosition = mul(worldToLocal, hips.position);

            float3 midway = (artDescriptor.leftArmDescriptor.upperArm.Transform.position + artDescriptor.rightArmDescriptor.upperArm.Transform.position) * 0.5f;
            float3 chestPosition = mul(worldToLocal, midway);

            var lowerNeckPosition = mul(worldToLocal, GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.Neck.C4Vertebra.position);

            proportions.neckProportions.lowerNeckEllipsoid.height = Mathf.Abs(lowerNeckPosition.y - chestPosition.y);

            float chestToHips = Mathf.Abs(chestPosition.y - hipPosition.y);

            proportions.spineProportions.upperChestEllipsoid.height = chestToHips * 0.35f;
            proportions.spineProportions.chestEllipsoid.height = chestToHips * 0.35f;
            proportions.spineProportions.spineEllipsoid.height = chestToHips * 0.3f;
            proportions.spineProportions.pelvisEllipsoid.height = chestToHips * 0.300495783f;

            Transform leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);

            Transform leftElbow = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);

            float3 leftHandPos = mul(worldToLocal, leftHand.position);
            float3 rightHandPos = mul(worldToLocal, rightHand.position);

            float roughWingspan = Mathf.Abs(leftHandPos.x - rightHandPos.x);

            float upperChestWidth = roughWingspan * 0.133002502f;
            float upperChestLength = upperChestWidth * 0.527777778f;

            proportions.spineProportions.upperChestEllipsoid.radius.x = upperChestWidth;
            proportions.spineProportions.upperChestEllipsoid.radius.y = upperChestLength;
            proportions.spineProportions.upperChestOffsetZ = 0f;

            proportions.spineProportions.chestEllipsoid.radius.x = upperChestWidth * 0.888888889f;
            proportions.spineProportions.chestEllipsoid.radius.y = upperChestLength * 1.3684212f;
            proportions.spineProportions.chestOffsetZ = upperChestLength * 0.315789507f;

            proportions.spineProportions.spineEllipsoid.radius.x = upperChestWidth * 0.666666667f;
            proportions.spineProportions.spineEllipsoid.radius.y = upperChestLength * 1.00000011f;
            proportions.spineProportions.spineOffsetZ = upperChestLength * 0.315789507f;

            proportions.spineProportions.pelvisEllipsoid.radius.x = upperChestWidth * 0.777777778f;
            proportions.spineProportions.pelvisEllipsoid.radius.y = upperChestLength * 1.05263169f;
            proportions.spineProportions.pelvisOffsetZ = upperChestLength * -0.210526338f;

            float3 floorPosition = mul(worldToLocal, transform.position);
            float hipToFloor = Mathf.Abs(hipPosition.y - floorPosition.y);

            float hipSeparation = proportions.spineProportions.pelvisEllipsoid.radius.x * 0.192857143f;
            float hipWidth = proportions.spineProportions.pelvisEllipsoid.radius.x * 0.414285714f;
            float hipLength = proportions.spineProportions.pelvisEllipsoid.radius.y * 0.75f;
            float hipHeight = hipToFloor * 0.429197829f;

            proportions.leftLegProportions.hipSeparationOffset = hipSeparation + hipWidth;
            proportions.leftLegProportions.hipEllipsoid.radius.x = hipWidth;
            proportions.leftLegProportions.hipEllipsoid.radius.y = hipLength;
            proportions.leftLegProportions.hipEllipsoid.height = hipHeight;

            proportions.leftLegProportions.kneeOffsetZ = hipLength * 0.2f;
            proportions.leftLegProportions.kneeEllipsoid.radius.x = hipWidth * 0.775862203f;
            proportions.leftLegProportions.kneeEllipsoid.radius.y = hipLength * 0.733333333f;
            proportions.leftLegProportions.kneeEllipsoid.height = hipHeight * 0.953488101f;

            proportions.leftLegProportions.ankleOffsetZ = hipLength * 0.386666452f;
            proportions.leftLegProportions.ankleEllipsoid.radius.x = hipWidth * 0.517241468f;
            proportions.leftLegProportions.ankleEllipsoid.radius.y = hipLength * 0.533333001f;
            proportions.leftLegProportions.ankleEllipsoid.height = hipHeight * 0.23255814f;

            proportions.leftLegProportions.toeOffset = new float3(hipWidth * 0.344827467f, hipHeight * 0.0511627907f, hipLength * 1.6f);
            proportions.leftLegProportions.toeEllipsoid.radius.x = hipWidth * 0.775862203f;
            proportions.leftLegProportions.toeEllipsoid.radius.y = hipLength * 0.933333333f;
            proportions.leftLegProportions.toeEllipsoid.height = hipHeight * 0.0906976243f;

            proportions.rightLegProportions = proportions.leftLegProportions;

            // 1.001869
            float armWingspan = roughWingspan * 0.740279026f;

            proportions.leftArmProportions.clavicleEllipsoid.radius.x = armWingspan * 0.0698694141f;
            proportions.leftArmProportions.clavicleEllipsoid.radius.y = armWingspan * 0.00998134487f;
            proportions.leftArmProportions.clavicleEllipsoid.height = armWingspan * 0.0199626897f;
            proportions.leftArmProportions.clavicleSeparation = armWingspan * 0.00998134487f;

            proportions.leftArmProportions.shoulderBladeEllipsoid.radius.x = armWingspan * 0.119776138f;
            proportions.leftArmProportions.shoulderBladeEllipsoid.radius.y = armWingspan * 0.0598880692f;
            proportions.leftArmProportions.shoulderBladeEllipsoid.height = armWingspan * 0.0299440346f;

            proportions.leftArmProportions.upperArmEllipsoid.radius.x = armWingspan * 0.0598880692f;
            proportions.leftArmProportions.upperArmEllipsoid.radius.y = armWingspan * 0.0459141864f;
            proportions.leftArmProportions.upperArmEllipsoid.height = armWingspan * 0.255522429f;

            proportions.leftArmProportions.elbowEllipsoid.radius.x = armWingspan * 0.034934707f;
            proportions.leftArmProportions.elbowEllipsoid.radius.y = armWingspan * 0.0429197829f;

            if (leftHand && leftElbow) {
                proportions.leftArmProportions.elbowEllipsoid.height = distance(leftHand.position, leftElbow.position);
            }
            else {
                proportions.leftArmProportions.elbowEllipsoid.height = armWingspan * 0.234561604f;
            }

            proportions.leftArmProportions.handProportions.wristEllipsoid.radius.x = armWingspan * 0.0259514967f;
            proportions.leftArmProportions.handProportions.wristEllipsoid.radius.y = armWingspan * 0.0199626897f;
            proportions.leftArmProportions.handProportions.wristEllipsoid.height = armWingspan * 0.106014858f;

            proportions.leftArmProportions.handProportions.knuckleEllipsoid.radius.x = armWingspan * 0.0478259533f;
            proportions.leftArmProportions.handProportions.knuckleEllipsoid.radius.y = armWingspan * 0.0202377656f;
            proportions.leftArmProportions.handProportions.knuckleEllipsoid.height = armWingspan * 0.108808038f;

            proportions.rightArmProportions = proportions.leftArmProportions;

            EditorCalculateNeck();
            EditorCalculateSpine();
            EditorCalculateArms();
            EditorCalculateLegs();

            EditorRefreshAvatar();
        }

        public void EditorCalculateLegs()
        {
            float3? eyeCenterRaw = EditorGetEyeCenter();
            if (!eyeCenterRaw.HasValue)
            {
                Debug.LogWarning($"Avatar {name} is missing an eye center! Please add an Eye Center Override before calculating proportions!", this);
                return;
            }

            float3 eyeCenter = eyeCenterRaw.Value;

            EditorRefreshAvatar();
            GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.Neck.EyeCenter.position = eyeCenter;

            HumanoidHelper.CalculateLeg(ref proportions.leftLegProportions, GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.LeftLeg, artDescriptor.leftLegDescriptor, transform); ;
            HumanoidHelper.CalculateLeg(ref proportions.rightLegProportions, GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.RightLeg, artDescriptor.rightLegDescriptor, transform); ;
        }

        public void EditorCalculateNeck()
        {
            float3? eyeCenterRaw = EditorGetEyeCenter();
            if (!eyeCenterRaw.HasValue)
            {
                Debug.LogWarning($"Avatar {name} is missing an eye center! Please add an Eye Center Override before calculating proportions!", this);
                return;
            }

            float3 eyeCenter = eyeCenterRaw.Value;

            EditorRefreshAvatar();
            GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.Neck.EyeCenter.position = eyeCenter;

            HumanoidHelper.CalculateNeck(ref proportions.neckProportions, GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton, artDescriptor);
        }

        public void EditorCalculateSpine() {
            float3? eyeCenterRaw = EditorGetEyeCenter();
            if (!eyeCenterRaw.HasValue)
            {
                Debug.LogWarning($"Avatar {name} is missing an eye center! Please add an Eye Center Override before calculating proportions!", this);
                return;
            }

            float3 eyeCenter = eyeCenterRaw.Value;

            EditorRefreshAvatar();
            GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.Neck.EyeCenter.position = eyeCenter;

            HumanoidHelper.CalculateSpine(ref proportions.spineProportions, GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton, artDescriptor);
        }

        public void EditorCalculateArms() {
            float3? eyeCenterRaw = EditorGetEyeCenter();
            if (!eyeCenterRaw.HasValue)
            {
                Debug.LogWarning($"Avatar {name} is missing an eye center! Please add an Eye Center Override before calculating proportions!", this);
                return;
            }

            float3 eyeCenter = eyeCenterRaw.Value;

            EditorRefreshAvatar();
            GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.Neck.EyeCenter.position = eyeCenter;

            HumanoidHelper.CalculateArm(ref proportions.leftArmProportions, GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.LeftArm, artDescriptor.leftArmDescriptor);
            HumanoidHelper.CalculateArm(ref proportions.rightArmProportions, GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.RightArm, artDescriptor.rightArmDescriptor);
        }

        [ContextMenu("Auto Fill Bone Transforms from Animator")]
        public void EditorAutoFillBoneTransforms() {
            Undo.RecordObject(this, "Auto Fill Bone Transforms from Animator");

            artDescriptor = new HumanoidArtDescriptor();
            artDescriptor.AutoFillBones(animator);
        }
#endif
    }
}
