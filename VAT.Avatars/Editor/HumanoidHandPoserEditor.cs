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
        private SerializedProperty _handPoseAsset;

        private void OnEnable() {
            _poser = target as HumanoidHandPoser;
            _handPoseData = serializedObject.FindProperty(nameof(_poser.handPoseData));
            _handPoseAsset = serializedObject.FindProperty(nameof(_poser.handPoseAsset));
        }

        private Mesh GeneratePreviewMesh()
        {
            var skinnedMeshRenderer = _poser.GetComponentInChildren<SkinnedMeshRenderer>();
            var copy = Instantiate(skinnedMeshRenderer);

            var palm = _poser.Hand.GetPointOnPalm(Vector2.up);
            copy.transform.SetPositionAndRotation(palm.position, palm.rotation);

            Mesh mesh = new();
            copy.BakeMesh(mesh);

            DestroyImmediate(copy.gameObject);

            return mesh;
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField("Humanoid Hand Poser", EditorStyles.whiteLargeLabel);

            GUILayout.Space(5);

            if (_poser.Generated) {
                EditorGUILayout.HelpBox("No issues found!", MessageType.Info);

                if (GUILayout.Button("Reset Hand Pose")) {
                    _poser.Hand.NeutralPose();
                }

                if (GUILayout.Button("Save to Asset"))
                {
                    var previewMesh = GeneratePreviewMesh();
                    previewMesh.name = "Preview Mesh";
                    AssetDatabase.AddObjectToAsset(previewMesh, _poser.handPoseAsset);

                    _poser.handPoseAsset.previewMesh = previewMesh;
                }

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(_handPoseData, true);

                EditorGUILayout.PropertyField(_handPoseAsset, true);

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

                _poser.Solve();
            }
        }
    }
}
