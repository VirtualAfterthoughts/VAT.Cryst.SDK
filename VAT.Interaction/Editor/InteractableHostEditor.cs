using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(InteractableHost))]
    public class InteractableHostEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var host = (InteractableHost)target;

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Host Validation", EditorStyles.boldLabel);

            if (ValidateHost(host))
            {
                EditorGUILayout.HelpBox("No issues found!", MessageType.Info);
            }
        }

        private bool ValidateHost(InteractableHost host)
        {
            if (host.GetComponent<Rigidbody>())
            {
                return true;
            }

            if (host.transform.parent == null)
            {
                return true;
            }

            var parentHost = host.transform.parent.GetComponentInParent<InteractableHost>(true);

            if (parentHost == null) 
            { 
                return true;
            }

            if (parentHost.GetComponent<Rigidbody>() != null) {
                EditorGUILayout.HelpBox("This Interactable Host appears to be extraneous, as there is already a host on the above Rigidbody.", MessageType.Warning);

                if (GUILayout.Button("Remove Interactable Host"))
                {
                    Undo.DestroyObjectImmediate(host);
                }

                return false;
            }

            return true;
        }
    }
}
