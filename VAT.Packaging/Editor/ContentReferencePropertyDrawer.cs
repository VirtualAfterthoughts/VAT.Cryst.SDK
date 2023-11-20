using System;

using UnityEngine;

using UnityEditor;

using VAT.Shared.Editor;

namespace VAT.Packaging.Editor
{
    [CustomPropertyDrawer(typeof(ContentReference), true)]
    public class ContentReferencePropertyDrawer : PropertyDrawer
    {
        public bool isDrawingAddress = false;

        private Type contentType = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            contentType = (property.GetPropertyInstance() as ContentReference).EditorContentType;

            var addressProperty = property.FindPropertyRelative("_address").FindPropertyRelative("_id");

            EditorGUI.BeginProperty(position, label, property);
            position.width -= 24;

            if (isDrawingAddress)
            {
                OnDrawAddress(position, label, addressProperty);
            }
            else
            {
                OnDrawContent(position, label, addressProperty);
            }

            OnDrawAddressToggle(position, addressProperty);
            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnDrawAddress(Rect position, GUIContent label, SerializedProperty addressProperty)
        {
            string result = EditorGUI.TextField(position, label, addressProperty.stringValue);
            addressProperty.stringValue = result;
        }

        protected virtual void OnDrawContent(Rect position, GUIContent label, SerializedProperty addressProperty)
        {
            var address = addressProperty.stringValue;
            AssetPackager.Instance.TryGetContent<Content>(address, out var content);

            EditorGUI.BeginChangeCheck();

            content = EditorGUI.ObjectField(position, label, content, contentType, false) as Content;

            if (EditorGUI.EndChangeCheck())
            {
                addressProperty.stringValue = content ? content.Address : Address.EMPTY;
            }
        }

        protected void OnDrawAddressToggle(Rect position, SerializedProperty addressProperty)
        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position.x += position.width + 24;
            position.width = position.height = EditorGUI.GetPropertyHeight(addressProperty);
            position.x -= position.width;
            isDrawingAddress = EditorGUI.Toggle(position, isDrawingAddress, EditorStyles.radioButton);
            EditorGUI.indentLevel = indent;
        }
    }
}
