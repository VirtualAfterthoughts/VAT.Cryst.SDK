using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Posing;
using VAT.Avatars.Skeletal;
using VAT.Shared.Data;

namespace VAT.Avatars.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine.XR;
    using VAT.Characters;
    using VAT.Cryst.Game;
    using VAT.Input;
    using VAT.Interaction;
    using VAT.Shared.Extensions;

    [CustomEditor(typeof(HumanoidHandPoser))]
    public sealed class HumanoidHandPoserEditor : Editor
    {
        private const float _handleSize = 0.005f;

        private HumanoidHandPoser _poser;
        private DataBone _selectedBone = null;

        private SerializedProperty _handPoseData;

        private SerializedProperty _selectedPose;
        private SerializedProperty _targetGrip;

        private void OnEnable() {
            _poser = target as HumanoidHandPoser;
            _handPoseData = serializedObject.FindProperty(nameof(_poser.handPoseData));

            _selectedPose = serializedObject.FindProperty(nameof(_poser.selectedPose));
            _targetGrip = serializedObject.FindProperty(nameof(_poser.targetGrip));
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField("Humanoid Hand Poser", EditorStyles.whiteLargeLabel);

            GUILayout.Space(5);

            if (_poser.Generated) {
                EditorGUILayout.HelpBox("No issues found!", MessageType.Info);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_selectedPose);

                bool changedPose = EditorGUI.EndChangeCheck();

                EditorGUILayout.PropertyField(_targetGrip);

                GUILayout.FlexibleSpace();

                EditorGUILayout.LabelField("Resetting", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

                GUILayout.FlexibleSpace();

                if (_poser.selectedPose != null)
                {
                    DrawResetToSelected();
                }

                DrawResetToNeutral();

                GUILayout.FlexibleSpace();

                EditorGUILayout.LabelField("Saving", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

                GUILayout.FlexibleSpace();

                if (_poser.selectedPose != null) {
                    DrawSaveToPose();
                }

                DrawCreateNewPose();

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(_handPoseData, true);

                if (EditorGUI.EndChangeCheck()) {
                    _poser.Solve();
                }

                serializedObject.ApplyModifiedProperties();

                if (changedPose)
                {
                    Undo.RecordObject(_poser, "Change Selected Pose");

                    if (_poser.selectedPose != null)
                    {
                        _poser.handPoseData = HandPoseCreator.Clone(_poser.selectedPose.data);
                    }
                    else
                    {
                        _poser.ResetToNeutral();
                    }
                }
            }
            else {
                EditorGUILayout.HelpBox("This poser is invalid! Please generate it from an avatar.", MessageType.Error);
            }
        }

        private void DrawCreateNewPose()
        {
            if (GUILayout.Button("Create New Pose"))
            {
                var path = EditorUtility.SaveFilePanel("Create New Pose", "Assets", "New Hand Pose", "asset");

                if (string.IsNullOrWhiteSpace(path))
                    return;

                HandPose pose = CreateInstance<HandPose>();
                pose.data = _poser.handPoseData;

                AssetDatabase.CreateAsset(pose, CrystAssetManager.GetProjectRelativePath(path));

                _poser.selectedPose = pose;
                EditorUtility.SetDirty(_poser);
            }
        }

        private void DrawSaveToPose()
        {
            if (GUILayout.Button("Save To Pose"))
            {
                bool proceed = EditorUtility.DisplayDialog("Save To Pose", "This will overwrite the current selected pose data. Proceed?", "Save", "Cancel");

                if (proceed)
                {
                    _poser.selectedPose.data = HandPoseCreator.Clone(_poser.handPoseData);
                    EditorUtility.SetDirty(_poser.selectedPose);
                }
            }
        }

        private void DrawResetToSelected()
        {
            if (GUILayout.Button("Reset To Selected Pose"))
            {
                Undo.RecordObject(_poser, "Reset Hand Pose");

                _poser.handPoseData = HandPoseCreator.Clone(_poser.selectedPose.data);
            }
        }

        private void DrawResetToNeutral()
        {
            if (GUILayout.Button("Reset To Neutral Pose"))
            {
                Undo.RecordObject(_poser, "Reset Hand Pose");

                _poser.ResetToNeutral();
            }
        }

        private void DrawClickableGrips()
        {
            var style = new GUIStyle(GUI.skin.label) 
            { 
                alignment = TextAnchor.MiddleCenter,
            };

            style.normal.textColor = Color.cyan;

            var allGrips = FindObjectsOfType<Grip>();
            foreach (var grip in allGrips)
            {
                if (grip == _poser.targetGrip)
                    continue;

                Handles.Label(grip.transform.position + Vector3.up * _handleSize, $"{grip.GetType().Name} {grip.name}", style);

                bool pressed = Handles.Button(grip.transform.position, grip.transform.rotation, _handleSize, _handleSize / 2f, Handles.SphereHandleCap);

                if (pressed)
                {
                    Undo.RecordObject(_poser, "Change Grip");
                    _poser.targetGrip = grip;
                }
            }
        }

        public void OnSceneGUI() {
            if (_poser.Initiated) {
                DrawClickableGrips();

                Handles.color = new Color(255, 87, 51, 255) / 255;

                if (_poser.handPoseData.fingers != null && _poser.handPoseData.fingers.Length > 0)
                {
                    for (var i = 0; i < _poser.Hand.Fingers.Length; i++)
                    {
                        var (index, blend) = ArrayRemapper.RemapIndex(i, _poser.handPoseData.fingers.Length, _poser.Hand.Fingers.Length);

                        DrawFinger(_poser.Hand.Fingers[i], ref _poser.handPoseData.fingers[index]);
                    }
                }

                if (_poser.handPoseData.thumbs != null && _poser.handPoseData.thumbs.Length > 0)
                {
                    for (var i = 0; i < _poser.Hand.Thumbs.Length; i++)
                    {
                        var (index, blend) = ArrayRemapper.RemapIndex(i, _poser.handPoseData.thumbs.Length, _poser.Hand.Thumbs.Length);

                        DrawThumb(_poser.Hand.Thumbs[i], ref _poser.handPoseData.thumbs[index]);
                    }
                }

                DrawWrist();

                _poser.Solve();
            }
        }

        private void DrawWrist()
        {
            var grip = _poser.targetGrip;
            if (grip == null)
                return;

            Handles.color = Color.green;

            var grabberPoint = new AvatarGrabberPoint()
            {
                hand = _poser.Hand,
            };
            var worldTarget = grip.GetDefaultTargetInWorld(grabberPoint, _poser.handPoseData);

            var bone = _poser.Hand.Hand;

            if (_selectedBone == bone && Tools.current == Tool.None)
            {
                EditorGUI.BeginChangeCheck();

                var rotationOffset = _poser.handPoseData.rotationOffset.normalized;
                rotationOffset.ToAngleAxis(out var angle, out var axis);
                axis.x = -axis.x;
                axis.y = -axis.y;
                rotationOffset = Quaternion.AngleAxis(angle, axis);

                var newRotation = Handles.RotationHandle(worldTarget.rotation * rotationOffset, worldTarget.position);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_poser, "Modify Hand Pose Data");

                    var newOffset = Quaternion.Inverse(worldTarget.rotation) * newRotation;
                    newOffset.ToAngleAxis(out var newAngle, out var newAxis);
                    newAxis.x = -newAxis.x;
                    newAxis.y = -newAxis.y;
                    _poser.handPoseData.rotationOffset = Quaternion.AngleAxis(newAngle, newAxis);

                    _poser.SolveGrip();
                }
            }
            else
            {
                bool pressed = Handles.Button(worldTarget.position, worldTarget.rotation, _handleSize, _handleSize / 2f, Handles.SphereHandleCap);

                if (pressed)
                {
                    _selectedBone = bone;
                    Tools.current = Tool.None;
                }
            }
        }

        private void DrawFinger(HumanoidFinger finger, ref FingerPoseData fingerPose)
        {
            DrawBone(finger.Proximal, ref fingerPose.phalanges[0], out float? splay);
            DrawBone(finger.Middle, ref fingerPose.phalanges[1], out _);
            DrawBone(finger.Distal, ref fingerPose.phalanges[2], out _);

            if (splay.HasValue)
                fingerPose.splay = Mathf.Clamp(splay.Value, -1f, 1f);
        }

        private void DrawThumb(HumanoidThumb thumb, ref ThumbPoseData thumbPose)
        {
            //DrawBone(thumb.Proximal);
            DrawBone(thumb.Middle, ref thumbPose.phalanges[0], out _);
            DrawBone(thumb.Distal, ref thumbPose.phalanges[1], out _);
        }

        private void DrawBone(DataBone bone, ref PhalanxPoseData phalanx, out float? splay)
        {
            splay = null;

            if (_selectedBone == bone && Tools.current == Tool.None)
            {
                EditorGUI.BeginChangeCheck();
                var newRotation = Handles.RotationHandle(bone.rotation, bone.position);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_poser, "Modify Hand Pose Data");

                    var parent = bone.Parent;

                    // Splay
                    var parentUp = parent.up;
                    var splayRotation = Quaternion.FromToRotation(newRotation * Vector3.up, parentUp) * newRotation;

                    float splayAngle = Vector3.SignedAngle(parent.forward, splayRotation * Vector3.forward, -parentUp);
                    float inverseSplay = Mathf.InverseLerp(-30f, 30f, splayAngle);
                    float remappedSplay = inverseSplay * 2f - 1f;

                    // Curl
                    var parentRight = parent.right;
                    var curlRotation = Quaternion.FromToRotation(newRotation * Vector3.right, parentRight) * newRotation;

                    float signedAngle = Vector3.SignedAngle(parent.up, curlRotation * Vector3.up, parentRight);
                    float inverseLerp = Mathf.InverseLerp(-90f, 90f, signedAngle);
                    float remapped = inverseLerp * 2f - 1f;

                    splay = remappedSplay;
                    phalanx.curl = remapped;
                }
            }
            else
            {
                bool pressed = Handles.Button(bone.position, bone.rotation, _handleSize, _handleSize / 2f, Handles.SphereHandleCap);

                if (pressed)
                {
                    _selectedBone = bone;
                    Tools.current = Tool.None;
                }
            }
        }
    }
}
