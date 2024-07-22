using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Interaction.Entities;

namespace VAT.Interaction.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(CrystBody))]
    [CanEditMultipleObjects]
    public class CrystBodyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var body = (CrystBody)target;

            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.ObjectField("Rigidbody", body.Rigidbody, typeof(Rigidbody), true);

            EditorGUI.EndDisabledGroup();

            if (!Application.isPlaying)
            {
                CheckEditorChanges(body);
            }
            else
            {
                CheckRuntimeChanges(body);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void CheckRuntimeChanges(CrystBody body)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultInfo"));
        }

        private void CheckEditorChanges(CrystBody body)
        {
            // Get rigidbody
            if (!body.HasBody && body.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                serializedObject.FindProperty("_rigidbody").objectReferenceValue = rigidbody;
                body.Info.CopyTo(rigidbody);
            }

            // Update rigidbody info
            if (!body.HasBody)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultInfo"));

                if (GUILayout.Button("Create Rigidbody"))
                {
                    body.CreateBody();

                    serializedObject.FindProperty("_rigidbody").objectReferenceValue = body.Rigidbody;
                }
            }
            else
            {
                if (body.Info.HasChanged(body.Rigidbody))
                {
                    body.Info.CopyFrom(body.Rigidbody);

                    EditorUtility.SetDirty(body);
                }

                if (GUILayout.Button("Destroy Rigidbody"))
                {
                    DestroyImmediate(body.Rigidbody);

                    serializedObject.FindProperty("_rigidbody").objectReferenceValue = null;
                }
            }
        }
    }
}
