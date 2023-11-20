using System;

using UnityEngine;

using UnityEditor;

using VAT.Shared.Editor;

namespace VAT.Packaging.Editor
{
    [CustomPropertyDrawer(typeof(ContentReference), true)]
    public class ContentReferencePropertyDrawer : PropertyDrawer {
        public Content selectedContent = null;
        public bool isDrawingAddress = false;

        private bool _hasRan = false;
        private bool _hadValue = false;

        private Type contentType = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
        {
            contentType = (property.GetPropertyInstance() as ContentReference).EditorContentType;

            var addressProperty = property.FindPropertyRelative("_address").FindPropertyRelative("_id");

            EditorGUI.BeginProperty(position, label, property);
            position.width -= 24;

            if (isDrawingAddress) {
                OnDrawAddress(position, label, addressProperty);
            }
            else {
                OnDrawContent(position, label, addressProperty);
            }

            OnDrawAddressToggle(position, addressProperty);
            EditorGUI.EndProperty();
        }

        protected virtual void OnDrawAddress(Rect position, GUIContent label, SerializedProperty addressProperty) 
        {
            string result = EditorGUI.TextField(position, label, addressProperty.stringValue);
            addressProperty.stringValue = result;

            if (AssetPackager.IsReady) {
                AssetPackager.Instance.TryGetContent(result, out selectedContent);
            }
        }

        protected virtual void OnDrawContent(Rect position, GUIContent label, SerializedProperty addressProperty) 
        {
            selectedContent = EditorGUI.ObjectField(position, label, selectedContent, contentType, false) as Content;
            if (selectedContent)
                addressProperty.stringValue = selectedContent.Address.ID;
            else if (!_hasRan && AssetPackager.IsReady)
            {
                AssetPackager.Instance.TryGetContent(addressProperty.stringValue, out selectedContent);
                _hasRan = true;
            }
            else if (_hadValue)
            {
                addressProperty.stringValue = Address.EMPTY;
            }

            _hadValue = selectedContent != null;
        }

        protected void OnDrawAddressToggle(Rect position, SerializedProperty addressProperty) {
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
