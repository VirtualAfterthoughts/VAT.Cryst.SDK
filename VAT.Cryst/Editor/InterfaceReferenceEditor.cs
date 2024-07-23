using VAT.Cryst.Interfaces;

namespace VAT.Cryst.Editor
{
    using UnityEditor;

    using UnityEngine;

    using VAT.Shared.Editor;

    [CustomPropertyDrawer(typeof(InterfaceReference<>))]
    public class InterfaceReferenceEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetProperty = property.FindPropertyRelative("_target");

            var propertyType = property.GetPropertyInstance().GetType();
            var interfaceType = propertyType.GenericTypeArguments[0];

            EditorGUI.ObjectField(position, targetProperty, interfaceType, label);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
