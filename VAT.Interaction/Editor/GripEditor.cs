using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Interaction.Entities;

namespace VAT.Interaction.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(Grip), true)]
    public class GripEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var grip = (Grip)target;

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Grip Validation", EditorStyles.boldLabel);

            var body = grip.GetComponentInParent<CrystBody>(true);

            if (body == null)
            {
                EditorGUILayout.HelpBox("This Grip is missing a Cryst Body!", MessageType.Error);

                if (GUILayout.Button("Add Cryst Body"))
                {
                    var rb = grip.GetComponentInParent<Rigidbody>(true);
                    var root = rb ? rb.gameObject : grip.gameObject;

                    var newBody = root.AddComponent<CrystBody>();
                    Undo.RegisterCreatedObjectUndo(newBody, "Add Cryst Body");

                    Selection.activeGameObject = root;
                    EditorGUIUtility.PingObject(newBody);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No issues found!", MessageType.Info);
            }
        }
    }
}
