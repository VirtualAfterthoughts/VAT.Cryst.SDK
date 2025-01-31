using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

namespace VAT.Packaging.Editor
{
    [CustomEditor(typeof(Crystal))]
    [CanEditMultipleObjects]
    public class CrystalEditor : UnityEditor.Editor
    {
        private SerializedProperty _crystalInfo;
        private SerializedProperty _shards;

        private void OnEnable()
        {
            _crystalInfo = serializedObject.FindProperty("_crystalInfo");
            _shards = serializedObject.FindProperty("_shards");

            var crystal = serializedObject.targetObject as Crystal;
            crystal.OnValidate();
        }

        public override void OnInspectorGUI()
        {
            var crystal = serializedObject.targetObject as Crystal;

            serializedObject.Update();

            // Locked information
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Address", crystal.Address.ID);
            EditorGUI.EndDisabledGroup();

            // Basic information that can be updated
            EditorGUILayout.PropertyField(_crystalInfo);

            // Draw content list and content buttons
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_shards);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Add Static Shard", GUILayout.Width(120)))
            {
                StaticShardCreationWizard.Initialize(crystal);
            }

            if (GUILayout.Button("Add Data Shard", GUILayout.Width(120)))
            {
                DataShardCreationWizard.Initialize(crystal);
            }

            // Space and header
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Exporting Options", EditorStyles.whiteLargeLabel, GUILayout.Height(20));
            GUILayout.Space(20);

            // Draw build buttons
            if (GUILayout.Button("Pack for PC", GUILayout.Width(120)))
            {
                ExternalAssetPacker.PackCrystal(crystal, BuildTarget.StandaloneWindows64);
            }

            // Draw exporting buttons
            if (GUILayout.Button("Export as JSON", GUILayout.Width(120)))
            {
                PackageTools.ExportPackage(crystal);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
