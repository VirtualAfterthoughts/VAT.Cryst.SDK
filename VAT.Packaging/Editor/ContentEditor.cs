using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using VAT.Shared.Extensions;

namespace VAT.Packaging.Editor
{
    [CustomEditor(typeof(StaticContent), true)]
    [CanEditMultipleObjects]
    public class StaticContentEditor : UnityEditor.Editor
    {
        private SerializedProperty _contentInfo;
        private SerializedProperty _package;
        private SerializedProperty _mainAsset;

        protected virtual void OnEnable()
        {
            _contentInfo = serializedObject.FindProperty("_contentInfo");
            _package = serializedObject.FindProperty("_package");
            _mainAsset = serializedObject.FindProperty("_mainAsset");

            var content = serializedObject.targetObject as StaticContent;
            content.OnValidate();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var content = target as StaticContent;

            // Locked information
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_package);

            EditorGUILayout.TextField("Address", content.Address);
            EditorGUI.EndDisabledGroup();

            // Basic information that can be updated
            EditorGUILayout.PropertyField(_contentInfo);

            // Draw the assets
            if (_mainAsset != null)
            {
                EditorGUILayout.PropertyField(_mainAsset);
            }
            else
            {
                EditorGUILayout.HelpBox("Developer Note: No serializable property is detected with the name \"_mainAsset\". Please add a field under this name or create a custom editor.", MessageType.Error);
            }

            // Apply changes
            serializedObject.ApplyModifiedProperties();
        }
    }
}
