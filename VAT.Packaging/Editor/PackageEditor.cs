using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using VAT.Shared.Extensions;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

namespace VAT.Packaging.Editor
{
    [CustomEditor(typeof(Package))]
    [CanEditMultipleObjects]
    public class PackageEditor : UnityEditor.Editor {
        private SerializedProperty _packageInfo;
        private SerializedProperty _contents;

        private void OnEnable() {
            _packageInfo = serializedObject.FindProperty("_packageInfo");
            _contents = serializedObject.FindProperty("_contents");

            var package = serializedObject.targetObject as Package;
            package.OnValidate();
        }

        public override void OnInspectorGUI() {
            var package = serializedObject.targetObject as Package;

            serializedObject.Update();

            // Locked information
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Address", package.Address);
            EditorGUI.EndDisabledGroup();

            // Basic information that can be updated
            EditorGUILayout.PropertyField(_packageInfo);

            // Draw content list and content buttons
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_contents);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Add Static Content", GUILayout.Width(120))) {
                StaticContentCreationWizard.Initialize(package);
            }

            // Space and header
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Exporting Options", EditorStyles.whiteLargeLabel, GUILayout.Height(20));
            GUILayout.Space(20);

            // Draw build buttons
            if (GUILayout.Button("Pack for PC", GUILayout.Width(120))) {
                ExternalAssetPacker.PackPackage(package, BuildTarget.StandaloneWindows64);
            }

            // Draw exporting buttons
            if (GUILayout.Button("Export as JSON", GUILayout.Width(120))) {
                PackageTools.ExportPackage(package);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
