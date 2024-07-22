using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Interaction.Entities;

namespace VAT.Interaction.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(CrystEntity))]
    public class CrystEntityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_root"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_bodies"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_joints"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
