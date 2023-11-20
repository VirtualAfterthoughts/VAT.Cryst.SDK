using Codice.CM.Common.Tree;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using VAT.Shared.Editor;

namespace VAT.Packaging.Editor
{
    [CustomPropertyDrawer(typeof(StaticCrystAsset), true)]
    public class StaticCrystAssetPropertyDrawer : PropertyDrawer
    {
        public bool isDrawingGUID = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var asset = property.GetPropertyInstance() as StaticCrystAsset;
            EditorGUI.BeginProperty(position, label, property);
            position.width -= 24;

            if (isDrawingGUID)
            {
                OnDrawGUID(position, label, property.serializedObject, asset);
            }
            else
            {
                OnDrawAsset(position, label, property.serializedObject, asset);
            }

            OnDrawAddressToggle(position, label);
            EditorGUI.EndProperty();
        }

        protected virtual void OnDrawGUID(Rect position, GUIContent label, SerializedObject serializedObject, StaticCrystAsset asset)
        {
            EditorGUI.BeginChangeCheck();
            string result = EditorGUI.TextField(position, label, asset.AssetGUID);
            
            if (EditorGUI.EndChangeCheck())
            {
                asset.ValidateGUID(result);
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
        }

        protected virtual void OnDrawAsset(Rect position, GUIContent label, SerializedObject serializedObject, StaticCrystAsset asset)
        {
            EditorGUI.BeginChangeCheck();
            var result = EditorGUI.ObjectField(position, label, asset.EditorAsset, asset.AssetType, false);

            if (EditorGUI.EndChangeCheck())
            {
                asset.ValidateGUID(result);
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
        }

        protected void OnDrawAddressToggle(Rect position, GUIContent label)
        {
            SerializedPropertyType type = isDrawingGUID switch
            {
                true => SerializedPropertyType.ObjectReference,
                false => SerializedPropertyType.String
            };
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position.x += position.width + 24;
            position.width = position.height = EditorGUI.GetPropertyHeight(type, label);
            position.x -= position.width;
            isDrawingGUID = EditorGUI.Toggle(position, isDrawingGUID, EditorStyles.radioButton);
            EditorGUI.indentLevel = indent;
        }
    }
}
