using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Integumentary;
using VAT.Avatars.Skeletal;
using VAT.Avatars.Muscular;
using VAT.Avatars.Posing;
using VAT.Avatars.Proportions;

using VAT.Shared.Data;
using VAT.Shared.Extensions;

using static Unity.Mathematics.math;

namespace VAT.Avatars.Editor {
    using Unity.Mathematics;

    using UnityEditor;

    [CustomEditor(typeof(HumanoidAvatar), true)]
    public sealed class HumanoidAvatarEditor : Editor {
        private enum BoneGroup {
            NECK = 1 << 0,
            SPINE = 1 << 1,
            ARMS = 1 << 2,
            LEGS = 1 << 3,
        }

        private quaternion _worldToLocal = default;

        private HumanoidAvatar _avatar;

        private static bool _editingFoldout = true;
        private static bool _manualFoldout = false;

        private static bool _useSymmetry = true;
        private static BoneGroup _boneGroup = BoneGroup.NECK;
        private static bool _isLeft = false;

        private SerializedProperty _animator;
        private SerializedProperty _eyeCenterOverride;
        private SerializedProperty _proportions;
        private SerializedProperty _artDescriptor;

        private void OnEnable() {
            _avatar = target as HumanoidAvatar;

            _animator = serializedObject.FindProperty("animator");
            _eyeCenterOverride = serializedObject.FindProperty("eyeCenterOverride");
            _proportions = serializedObject.FindProperty("proportions");
            _artDescriptor = serializedObject.FindProperty("artDescriptor");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.LabelField("Humanoid Avatar", EditorStyles.whiteLargeLabel);

            GUILayout.Space(5);

            if (OnValidateAvatar(out var reasons)) {
                EditorGUILayout.HelpBox("No issues found!", MessageType.Info);
                
                EditorGUILayout.LabelField("Quick Actions", EditorStyles.whiteLargeLabel);
                
                GUILayout.Space(5);

                EditorGUILayout.HelpBox("If your Avatar is not in a T-Pose, either:\n" +
                    "1. Manually rotate the bones to match a T-Pose, or\n" +
                    "2. Use the \"Set Animator Pose\" button.", MessageType.Warning);

                if (GUILayout.Button("Set Animator Pose")) {
                    _avatar.EditorSetAnimatorPose();
                }

                if (GUILayout.Button("Auto Calculate Proportions")) {
                    _avatar.EditorCalculateProportions();
                }

                if (GUILayout.Button("Auto Fill Bone Transforms")) {
                    _avatar.EditorAutoFillBoneTransforms();
                }

                if (GUILayout.Button("Create Hand Poser")) {
                    HumanoidAvatar instance = Instantiate(_avatar);
                    instance.transform.SetPositionAndRotation(_avatar.transform.position, _avatar.transform.rotation);

                    instance.Initiate();
                    instance.EditorUpdateEyeCenter();

                    if (instance.TryCreateHandPoser(out var poser)) {
                        // Rename poser
                        instance.gameObject.hideFlags = HideFlags.DontSaveInEditor;
                        instance.gameObject.name = $"{_avatar.name} Hand Poser";
                        Transform avatarTransform = instance.transform;

                        // Hide the rest of the model
                        var handPoser = (HumanoidHandPoser)poser;
                        Transform hand = handPoser.descriptor.hand.transform;

                        hand.parent = avatarTransform;
                        avatarTransform.SetPositionAndRotation(hand.position, hand.rotation);
                        hand.localPosition = Vector3.zero;
                        hand.localRotation = Quaternion.identity;

                        Transform hips = instance.artDescriptor.spineDescriptor.hips.transform;
                        if (hips != null) {
                            hips.parent = hand;
                            hips.Reset();
                            hips.localScale = Vector3.zero;
                        }

                        foreach (var mesh in instance.GetComponentsInChildren<SkinnedMeshRenderer>(true)) {
                            mesh.rootBone = hand;
                        }

                        DestroyImmediate(instance);

                        Selection.SetActiveObjectWithContext(handPoser.gameObject, handPoser);
                        return;
                    }
                    else {
                        DestroyImmediate(instance.gameObject);
                    }
                }
            }
            else {
                foreach (var reason in reasons) {
                    EditorGUILayout.HelpBox(reason, MessageType.Error);
                }
            }

            GUILayout.Space(5);

            OnEditingFoldout();

            GUILayout.Space(10);

            OnManualFoldout();

            serializedObject.ApplyModifiedProperties();
        }

        private bool OnValidateAvatar(out List<string> reasons) {
            reasons = new List<string>();

            // Make sure we have an animator and it's humanoid
            if (!_avatar.animator) {
                reasons.Add("There is no animator set! Please add an animator!");
            }
            else if (!_avatar.animator.avatar) {
                reasons.Add("The animator does not have an avatar! Please generate an avatar in the mesh file!");
            }
            else if (!_avatar.animator.isHuman) {
                reasons.Add("The animator is not humanoid! Please mark it as humanoid in the mesh settings!");
            }

            // Check if we have an eye set
            if (!_avatar.EditorGetEyeCenter().HasValue) {
                reasons.Add("The Avatar has no eye center! Please assign an EyeCenterOverride!");
            }

            return reasons.Count <= 0;
        }

        private void OnEditingFoldout() {
            _editingFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_editingFoldout, "Editor Settings", EditorStyles.foldoutHeader);

            if (_editingFoldout)
            {
                EditorGUILayout.PropertyField(_animator);
                EditorGUILayout.PropertyField(_eyeCenterOverride);

                _useSymmetry = EditorGUILayout.Toggle("Use Symmetry", _useSymmetry);

                if (_boneGroup == BoneGroup.ARMS || _boneGroup == BoneGroup.LEGS)
                    _isLeft = EditorGUILayout.Toggle("Is Left", _isLeft);

                _boneGroup = (BoneGroup)EditorGUILayout.EnumPopup("Selected Bone Group", _boneGroup);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

        }

        private void OnManualFoldout() {
            _manualFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_manualFoldout, "Manual Settings", EditorStyles.foldoutHeader);
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (_manualFoldout)
            {
                EditorGUILayout.PropertyField(_proportions);
                EditorGUILayout.PropertyField(_artDescriptor);
            }
        }

        public void OnSceneGUI()
        {
            if (Application.isPlaying)
                return;

            _worldToLocal = inverse(_avatar.transform.rotation);

            switch (_boneGroup) {
                default:
                case BoneGroup.NECK:
                    DrawNeckHandles();
                    break;
                case BoneGroup.SPINE:
                    DrawChestHandles();
                    break;
                case BoneGroup.ARMS:
                    if (_isLeft)
                        DrawArmHandles(ref _avatar.proportions.leftArmProportions, ref _avatar.proportions.rightArmProportions, _avatar.GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.LeftArm);
                    else
                        DrawArmHandles(ref _avatar.proportions.rightArmProportions, ref _avatar.proportions.leftArmProportions, _avatar.GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.RightArm);
                    break;
                case BoneGroup.LEGS:
                    if (_isLeft)
                        DrawLegHandles(ref _avatar.proportions.leftLegProportions, ref _avatar.proportions.rightLegProportions, _avatar.GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.LeftLeg);
                    else
                        DrawLegHandles(ref _avatar.proportions.rightLegProportions, ref _avatar.proportions.leftLegProportions, _avatar.GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.RightLeg);
                    break;
            }

            // Always match avatar bones
            _avatar.EditorCalculateSpine();
            _avatar.EditorCalculateArms();
            _avatar.EditorCalculateLegs();

            if (_avatar.Initiated) {
                _avatar.WriteArtOffsets();
            }
        }

        private void DrawNeckHandles() {
            var dataSkeleton = _avatar.GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton;
            var neck = dataSkeleton.Neck;

            if (DrawOffset(neck.Skull.position, out var skullOffset))
            {
                Undo.RecordObject(_avatar, "Adjust Skull Offset");

                _avatar.proportions.neckProportions.skullYOffset += skullOffset.y;
                _avatar.EditorRefreshAvatar();
            }

            if (DrawOffset(neck.C1Vertebra.position, out var upperNeckOffset))
            {
                Undo.RecordObject(_avatar, "Adjust Upper Neck Offset");

                _avatar.proportions.neckProportions.upperNeckOffsetZ -= upperNeckOffset.z;
                _avatar.EditorRefreshAvatar();
            }

            if (DrawOffset(neck.C4Vertebra.position, out var lowerNeckOffset))
            {
                Undo.RecordObject(_avatar, "Adjust Lower Neck Offset");

                _avatar.proportions.neckProportions.lowerNeckOffsetZ -= lowerNeckOffset.z;
                _avatar.EditorRefreshAvatar();
            }

            var topTransform = dataSkeleton.Neck.Skull.Transform;
            topTransform.position += neck.Skull.up * _avatar.proportions.neckProportions.skullEllipsoid.height * 0.5f;
            DrawAvatarEllipse(ref _avatar.proportions.neckProportions.topEllipse, _avatar, topTransform, "Top Head");

            var foreheadTransform = dataSkeleton.Neck.Skull.Transform;
            foreheadTransform.position += neck.Skull.up * _avatar.proportions.neckProportions.skullEllipsoid.height * 0.3f;

            DrawAvatarEllipse(ref _avatar.proportions.neckProportions.foreheadEllipse, _avatar, foreheadTransform, "Forehead");

            DrawAvatarEllipsoid(ref _avatar.proportions.neckProportions.skullEllipsoid, _avatar, dataSkeleton.Neck.Skull.Transform, 0f, "Skull");

            var jawTransform = dataSkeleton.Neck.Skull.Transform;
            jawTransform.position -= neck.Skull.up * _avatar.proportions.neckProportions.skullEllipsoid.height * 0.3f;
            jawTransform.rotation *= Quaternion.AngleAxis(25f, neck.Skull.right);
            DrawAvatarEllipse(ref _avatar.proportions.neckProportions.jawEllipse, _avatar, jawTransform, "Jaw");

            DrawAvatarEllipsoid(ref _avatar.proportions.neckProportions.upperNeckEllipsoid, _avatar, dataSkeleton.Neck.C1Vertebra.Transform, -1f, "Upper Neck");
            DrawAvatarEllipsoid(ref _avatar.proportions.neckProportions.lowerNeckEllipsoid, _avatar, dataSkeleton.Neck.C4Vertebra.Transform, -1f, "Lower Neck");
        }

        private void DrawChestHandles() {
            var dataSkeleton = _avatar.GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton;
            var spine = dataSkeleton.Spine;

            if (DrawOffset(spine.T1Vertebra.position, out var upperChestOffset))
            {
                Undo.RecordObject(_avatar, "Adjust Upper Chest Offset");

                _avatar.proportions.spineProportions.upperChestOffsetZ += upperChestOffset.z;
                _avatar.EditorRefreshAvatar();
            }

            if (DrawOffset(spine.T7Vertebra.position, out var chestOffset))
            {
                Undo.RecordObject(_avatar, "Adjust Chest Offset");

                _avatar.proportions.spineProportions.chestOffsetZ += chestOffset.z;
                _avatar.EditorRefreshAvatar();
            }

            if (DrawOffset(spine.L1Vertebra.position, out var spineOffset))
            {
                Undo.RecordObject(_avatar, "Adjust Spine Offset");

                _avatar.proportions.spineProportions.spineOffsetZ += spineOffset.z;
                _avatar.EditorRefreshAvatar();
            }

            if (DrawOffset(spine.Sacrum.position, out var pelvisOffset))
            {
                Undo.RecordObject(_avatar, "Adjust Pelvis Offset");

                _avatar.proportions.spineProportions.pelvisOffsetZ += pelvisOffset.z;
                _avatar.EditorRefreshAvatar();
            }

            DrawAvatarEllipsoid(ref _avatar.proportions.spineProportions.upperChestEllipsoid, _avatar, spine.T1Vertebra.Transform, -1f, "Upper Chest");
            DrawAvatarEllipsoid(ref _avatar.proportions.spineProportions.chestEllipsoid, _avatar, spine.T7Vertebra.Transform, -1f, "Chest");
            DrawAvatarEllipsoid(ref _avatar.proportions.spineProportions.spineEllipsoid, _avatar, spine.L1Vertebra.Transform, -1f, "Spine");
            DrawAvatarEllipsoid(ref _avatar.proportions.spineProportions.pelvisEllipsoid, _avatar, spine.Sacrum.Transform, -1f, "Pelvis");
        }

        private void DrawLegHandles(ref HumanoidLegProportions proportions, ref HumanoidLegProportions otherProportions, HumanoidLeg leg) {
            if (DrawOffset(leg.Hip.position, out var hipOffset))
            {
                Undo.RecordObject(_avatar, "Adjust Hip Offset");

                proportions.hipSeparationOffset += hipOffset.x * (leg.isLeft ? -1f : 1f);
                proportions.hipSeparationOffset = max(0f, proportions.hipSeparationOffset);

                if (_useSymmetry)
                    otherProportions.hipSeparationOffset = proportions.hipSeparationOffset;

                _avatar.EditorRefreshAvatar();
            }

            if (DrawOffset(leg.Knee.position, out var kneeOffset))
            {
                Undo.RecordObject(_avatar, "Adjust Knee Offset");

                proportions.kneeOffsetZ -= kneeOffset.z;

                if (_useSymmetry)
                    otherProportions.kneeOffsetZ = proportions.kneeOffsetZ;

                _avatar.EditorRefreshAvatar();
            }

            if (DrawOffset(leg.Ankle.position, out var ankleOffset))
            {
                Undo.RecordObject(_avatar, "Adjust Ankle Offset");

                proportions.ankleOffsetZ -= ankleOffset.z;

                if (_useSymmetry)
                    otherProportions.ankleOffsetZ = proportions.ankleOffsetZ;

                _avatar.EditorRefreshAvatar();
            }

            if (DrawOffset(leg.Toe.position, out var toeOffset))
            {
                Undo.RecordObject(_avatar, "Adjust Toe Offset");

                if (leg.isLeft)
                    toeOffset.x *= -1f;

                proportions.toeOffset += toeOffset;

                if (_useSymmetry)
                    otherProportions.toeOffset = proportions.toeOffset;

                _avatar.EditorRefreshAvatar();
            }

            DrawAvatarEllipsoidSymmetry(ref proportions.hipEllipsoid, ref otherProportions.hipEllipsoid, _avatar, leg.Hip.Transform, -1f, "Hip", new float2(leg.isLeft ? -1f : 1f, -1f));
            DrawAvatarEllipsoidSymmetry(ref proportions.kneeEllipsoid, ref otherProportions.kneeEllipsoid, _avatar, leg.Knee.Transform, -1f, "Knee");
            DrawAvatarEllipsoidSymmetry(ref proportions.ankleEllipsoid, ref otherProportions.ankleEllipsoid, _avatar, leg.Ankle.Transform, -1f, "Ankle");
            DrawAvatarEllipsoidSymmetry(ref proportions.toeEllipsoid, ref otherProportions.toeEllipsoid, _avatar, leg.Toe.Transform, 0f, "Toe");
        }

        private void DrawArmHandles(ref HumanoidArmProportions proportions, ref HumanoidArmProportions otherProportions, HumanoidArm arm) {
            var clavicleTransform = arm.Clavicle.Transform;
            clavicleTransform.position += arm.Clavicle.right * (arm.isLeft ? -1f : 1f) * proportions.clavicleEllipsoid.radius.x;
            DrawAvatarEllipsoidSymmetry(ref proportions.clavicleEllipsoid, ref otherProportions.clavicleEllipsoid, _avatar, clavicleTransform, -1f, "Clavicle");

            DrawAvatarEllipsoidSymmetry(ref proportions.shoulderBladeEllipsoid, ref otherProportions.shoulderBladeEllipsoid, _avatar, arm.Scapula.Transform, -1f, "Shoulder Blade");

            var upperArmTransform = arm.UpperArm.Transform;
            upperArmTransform.rotation *= Quaternion.AngleAxis(-90f * (arm.isLeft ? -1f : 1f), arm.UpperArm.forward);
            DrawAvatarEllipsoidSymmetry(ref proportions.upperArmEllipsoid, ref otherProportions.upperArmEllipsoid, _avatar, upperArmTransform, -1f, "Upper Arm");

            var elbowTransform = arm.Elbow.Transform;
            elbowTransform.rotation *= Quaternion.AngleAxis(-90f * (arm.isLeft ? -1f : 1f), arm.Elbow.forward);
            DrawAvatarEllipsoidSymmetry(ref proportions.elbowEllipsoid, ref otherProportions.elbowEllipsoid, _avatar, elbowTransform, -1f, "Elbow");
        }

        private bool DrawOffset(float3 position, out float3 offset) {
            offset = float3.zero;

            EditorGUI.BeginChangeCheck();
            float3 newPosition = Handles.FreeMoveHandle(position, quaternion.identity, 0.01f, Vector3.zero, Handles.RectangleHandleCap);

            position = mul(_worldToLocal, position);
            newPosition = mul(_worldToLocal, newPosition);

            if (EditorGUI.EndChangeCheck()) {
                offset = newPosition - position;

                return true;
            }

            return false;
        }

        private void DrawAvatarEllipsoid(ref Ellipsoid ellipsoid, HumanoidAvatar _avatar, SimpleTransform transform, float offset, string name) {
            if (ellipsoid.DrawHandles(transform.position, transform.rotation, new float2(1f, -1f), out var radius, out var height, offset)) {
                Undo.RecordObject(_avatar, $"Adjust {name} Ellipsoid");

                ellipsoid.radius = radius;
                ellipsoid.height = height;

                _avatar.EditorRefreshAvatar();
            }
        }

        private void DrawAvatarEllipse(ref Ellipse ellipse, HumanoidAvatar _avatar, SimpleTransform transform, string name)
        {
            if (ellipse.DrawHandles(transform.position, transform.rotation, new float2(1f, -1f), out var radius))
            {
                Undo.RecordObject(_avatar, $"Adjust {name} Ellipse");

                ellipse.radius = radius;

                _avatar.EditorRefreshAvatar();
            }
        }

        private void DrawAvatarEllipsoidSymmetry(ref Ellipsoid ellipsoid, ref Ellipsoid otherEllipsoid, HumanoidAvatar _avatar, SimpleTransform transform, float offset, string name, float2? handleDirections = null)
        {
            float2 directions = new float2(1f, -1f);

            if (handleDirections.HasValue)
                directions = handleDirections.Value;

            if (ellipsoid.DrawHandles(transform.position, transform.rotation, directions, out var radius, out var height, offset))
            {
                Undo.RecordObject(_avatar, $"Adjust {name} Ellipsoid");

                ellipsoid.radius = radius;
                ellipsoid.height = height;

                if (_useSymmetry) {
                    otherEllipsoid.radius = radius;
                    otherEllipsoid.height = height;
                }

                _avatar.EditorRefreshAvatar();
            }
        }
    }
}