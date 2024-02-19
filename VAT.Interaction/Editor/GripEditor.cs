using System.Collections;
using System.Collections.Generic;

using UnityEngine;

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

            var host = grip.GetComponentInParent<InteractableHost>(true);

            if (host == null)
            {
                EditorGUILayout.HelpBox("This Grip is missing an Interactable Host!", MessageType.Error);

                if (GUILayout.Button("Add Interactable Host"))
                {
                    var rb = grip.GetComponentInParent<Rigidbody>(true);
                    var root = rb ? rb.gameObject : grip.gameObject;

                    var newHost = root.AddComponent<InteractableHost>();
                    Undo.RegisterCreatedObjectUndo(newHost, "Add Interactable Host");

                    Selection.activeGameObject = root;
                    EditorGUIUtility.PingObject(newHost);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No issues found!", MessageType.Info);
            }
        }
    }
}
