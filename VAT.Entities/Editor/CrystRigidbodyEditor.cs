using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Entities.PhysX;

namespace VAT.Entities.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(CrystRigidbody))]
    public class CrystRigidbodyEditor : Editor
    {
        private SerializedProperty _rigidbody;
        private SerializedProperty _info;

        private void Awake()
        {
            _rigidbody = serializedObject.FindProperty("_rigidbody");
            _info = serializedObject.FindProperty("_info");
        }

        public override void OnInspectorGUI()
        {
            if (_rigidbody == null || _info == null)
            {
                return;
            }

            var crystRigidbody = (CrystRigidbody)target;

            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(_rigidbody);

            EditorGUI.EndDisabledGroup();

            if (crystRigidbody.Rigidbody == null)
            {
                EditorGUILayout.PropertyField(_info);
            }
        }
    }
}
