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
            var guidProperty = property.FindPropertyRelative("_guid");

            EditorGUI.BeginProperty(position, label, property);
            position.width -= 24;

            if (isDrawingGUID)
            {
                OnDrawGUID(position, label, guidProperty, asset);
            }
            else
            {
                OnDrawAsset(position, label, guidProperty, asset);
            }

            OnDrawAddressToggle(position, label);
            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnDrawGUID(Rect position, GUIContent label, SerializedProperty guidProperty, StaticCrystAsset asset)
        {
            EditorGUI.BeginChangeCheck();
            string result = EditorGUI.TextField(position, label, guidProperty.stringValue);

            if (EditorGUI.EndChangeCheck())
            {
                guidProperty.stringValue = result;
                asset.ValidateGUID();
            }
        }

        protected virtual void OnDrawAsset(Rect position, GUIContent label, SerializedProperty guidProperty, StaticCrystAsset asset)
        {
            EditorGUI.BeginChangeCheck();
            var result = EditorGUI.ObjectField(position, label, asset.EditorAsset, asset.AssetType, false);

            if (EditorGUI.EndChangeCheck())
            {
                if (result == null)
                {
                    guidProperty.stringValue = null;
                }
                else if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(result, out string guid, out long _))
                {
                    guidProperty.stringValue = guid;
                }

                asset.ValidateGUID();
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
