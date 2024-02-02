using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Posing;
using VAT.Avatars.Skeletal;
using VAT.Shared.Data;

namespace VAT.Avatars.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(HumanoidHandPoser))]
    public sealed class HumanoidHandPoserEditor : Editor
    {
        private HumanoidHandPoser _poser;
        private int? _fingerIndex = null;

        private SerializedProperty _handPoseData;

        private void OnEnable() {
            _poser = target as HumanoidHandPoser;
            _handPoseData = serializedObject.FindProperty(nameof(_poser.handPoseData));
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField("Humanoid Hand Poser", EditorStyles.whiteLargeLabel);

            GUILayout.Space(5);

            if (_poser.Generated) {
                EditorGUILayout.HelpBox("No issues found!", MessageType.Info);

                if (GUILayout.Button("Reset Hand Pose")) {
                    _poser.Hand.NeutralPose();
                }

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(_handPoseData, true);

                if (EditorGUI.EndChangeCheck()) {
                    _poser.Solve();
                }
            }
            else {
                EditorGUILayout.HelpBox("This poser is invalid! Please generate it from an avatar.", MessageType.Error);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI() {
            if (_poser.Initiated) {
                Handles.color = new Color(255, 87, 51, 255) / 255;

                for (var i = 0; i < _poser.Hand.Fingers.Length; i++) {
                    DrawFinger(i);
                }

                _poser.Solve();
            }
        }

        private void DrawFinger(int i) {
            var finger = _poser.Hand.Fingers[i];

            if (i == _fingerIndex) {
                ShowFinger(finger);
            }
            else {
                float size = 0.005f;
                var button = Handles.Button(finger.End.position, finger.End.rotation, size, size * 2f, Handles.SphereHandleCap);
                if (button) {
                    _fingerIndex = i;
                }
            }
        }

        private void ShowFinger(HumanoidFinger finger)
        {
            var target = finger.NeutralEndBone.Transform(finger.openPoint);

            Vector3 position = target.position;
            Quaternion rotation = target.rotation;

            EditorGUI.BeginChangeCheck();

            Handles.TransformHandle(ref position, ref rotation);

            if (EditorGUI.EndChangeCheck())
            {
                rotation.Normalize();
                finger.openPoint = finger.NeutralEndBone.InverseTransform(SimpleTransform.Create(position, rotation));
            }
        }
    }
}
