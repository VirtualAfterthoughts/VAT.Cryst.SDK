using System.Linq;

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

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_colliders"));

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
            // Check colliders
            CheckColliders(body);

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

        private void CheckColliders(CrystBody body)
        {
            var setColliders = body.Colliders;
            var foundColliders = body.FindCollidersInChildren();

            if (setColliders.Length != foundColliders.Length)
            {
                DrawColliderValidation(body);
                return;
            }

            foreach (var collider in foundColliders)
            {
                if (!setColliders.Contains(collider))
                {
                    DrawColliderValidation(body);
                    return;
                }
            }
        }

        private void DrawColliderValidation(CrystBody body)
        {
            EditorGUILayout.HelpBox("The colliders on this Cryst Body are no longer accurate, and need to be collected!", MessageType.Warning);

            if (GUILayout.Button("Collect Colliders"))
            {
                body.CollectColliders();

                EditorUtility.SetDirty(body);
            }

            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        }
    }
}
