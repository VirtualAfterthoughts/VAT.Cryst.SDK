using System;

using UnityEngine;

using UnityEditor;

using VAT.Shared.Editor;

namespace VAT.Packaging.Editor
{
    [CustomPropertyDrawer(typeof(ShardReference), true)]
    public class ShardReferencePropertyDrawer : PropertyDrawer
    {
        public bool isDrawingAddress = false;

        private Type shardType = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            shardType = (property.GetPropertyInstance() as ShardReference).EditorShardType;

            var addressProperty = property.FindPropertyRelative("_address").FindPropertyRelative("_id");

            EditorGUI.BeginProperty(position, label, property);
            position.width -= 24;

            if (isDrawingAddress)
            {
                OnDrawAddress(position, label, addressProperty);
            }
            else
            {
                OnDrawShard(position, label, addressProperty);
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

        protected virtual void OnDrawShard(Rect position, GUIContent label, SerializedProperty addressProperty)
        {
            var address = addressProperty.stringValue;
            AssetPackager.Instance.TryGetShard<Shard>(new(address), out var content);

            EditorGUI.BeginChangeCheck();

            content = EditorGUI.ObjectField(position, label, content, shardType, false) as Shard;

            if (EditorGUI.EndChangeCheck())
            {
                addressProperty.stringValue = content ? content.Address.ID : Address.EMPTY;
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
