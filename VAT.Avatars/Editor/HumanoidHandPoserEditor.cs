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
        private const float _handleSize = 0.005f;

        private HumanoidHandPoser _poser;
        private DataBone _selectedBone = null;

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
                    Undo.RecordObject(_poser, "Reset Hand Pose");

                    _poser.handPoseData = new HandPoseData()
                    {
                        fingers = HandPoseCreator.CreateFingers(_poser.handPoseData.fingers.Length),
                        thumbs = HandPoseCreator.CreateThumbs(_poser.handPoseData.thumbs.Length),
                    };
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

                for (var i = 0; i < _poser.Hand.Fingers.Length; i++)
                {
                    DrawFinger(_poser.Hand.Fingers[i], ref _poser.handPoseData.fingers[i]);
                }

                for (var i = 0; i < _poser.Hand.Thumbs
                    .Length; i++)
                {
                    DrawThumb(_poser.Hand.Thumbs[i], ref _poser.handPoseData.thumbs[i]);
                }

                _poser.Solve();
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
