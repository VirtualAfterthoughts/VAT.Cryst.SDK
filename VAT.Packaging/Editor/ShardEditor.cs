using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

namespace VAT.Packaging.Editor
{
    [CustomEditor(typeof(DataShard), true)]
    [CanEditMultipleObjects]
    public class DataShardEditor : UnityEditor.Editor
    {
        private SerializedProperty _shardInfo;

        protected virtual void OnEnable()
        {
            _shardInfo = serializedObject.FindProperty("_shardInfo");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var shard = target as DataShard;

            // Locked information
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Crystal", shard.Crystal, typeof(Crystal), true);

            EditorGUILayout.TextField("Address", shard.Address.ID);
            EditorGUI.EndDisabledGroup();

            // Basic information that can be updated
            EditorGUILayout.PropertyField(_shardInfo);

            // Draw data
            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

            shard.OnEditorInspectorGUI(serializedObject);

            GUILayout.FlexibleSpace();

            // Apply changes
            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(StaticShard), true)]
    [CanEditMultipleObjects]
    public class StaticShardEditor : UnityEditor.Editor
    {
        private SerializedProperty _shardInfo;
        private SerializedProperty _mainAsset;

        protected virtual void OnEnable()
        {
            _shardInfo = serializedObject.FindProperty("_shardInfo");
            _mainAsset = serializedObject.FindProperty("_mainAsset");

            var shard = serializedObject.targetObject as StaticShard;
            shard.OnValidate();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var shard = target as StaticShard;

            // Locked information
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Crystal", shard.StaticCrystal, typeof(Crystal), true);

            EditorGUILayout.TextField("Address", shard.Address.ID);
            EditorGUI.EndDisabledGroup();

            // Basic information that can be updated
            EditorGUILayout.PropertyField(_shardInfo);

            // Draw the assets
            if (_mainAsset != null)
            {
                EditorGUILayout.PropertyField(_mainAsset);
            }
            else
            {
                EditorGUILayout.HelpBox("Developer Note: No serializable property is detected with the name \"_mainAsset\". Please add a field under this name or create a custom editor.", MessageType.Error);
            }

            // Draw buttons
            GUILayout.Space(5);

            if (GUILayout.Button("Generate Packed Assets"))
            {
                shard.GeneratePackedAssets();
                EditorUtility.SetDirty(shard);
            }

            // Draw other properties
            OnDrawExtraProperties();

            // Apply changes
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnDrawExtraProperties() { }
    }

    [CustomEditor(typeof(StaticGameObjectShard), true)]
    [CanEditMultipleObjects]
    public class StaticGameObjectShardEditor : StaticShardEditor
    {
        private MeshPreview _meshPreview = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            var shard = target as StaticGameObjectShard;

            if (shard.PreviewMesh?.EditorAssetT != null)
            {
                _meshPreview = new MeshPreview(shard.PreviewMesh.EditorAssetT);
            }
        }

        protected override void OnDrawExtraProperties()
        {
            GUILayout.Space(5);

            var shard = target as StaticGameObjectShard;

            if (shard.PreviewMesh?.EditorAssetT != null)
            {
                if (_meshPreview == null)
                {
                    _meshPreview = new MeshPreview(shard.PreviewMesh.EditorAssetT);
                }
                else
                {
                    _meshPreview.mesh = shard.PreviewMesh.EditorAssetT;
                }
            }

            if (_meshPreview != null)
            {
                var rect = GUILayoutUtility.GetRect(1, 200);
                _meshPreview.OnPreviewGUI(rect, "TextField");
            }
        }

        protected void OnDisable()
        {
            if (_meshPreview != null)
            {
                _meshPreview.Dispose();
                _meshPreview = null;
            }
        }
    }

    [CustomEditor(typeof(StaticLevelShard), true)]
    [CanEditMultipleObjects]
    public class StaticLevelShardEditor : StaticShardEditor
    {
        private SerializedProperty _chunkScenes;

        protected override void OnEnable()
        {
            _chunkScenes = serializedObject.FindProperty("_chunkScenes");

            base.OnEnable();
        }

        protected override void OnDrawExtraProperties()
        {
            EditorGUILayout.PropertyField(_chunkScenes);
        }
    }
}
