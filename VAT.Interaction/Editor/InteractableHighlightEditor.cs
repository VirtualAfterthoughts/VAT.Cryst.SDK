using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(InteractableHighlight), true)]
    public class InteractableHighlightEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var highlight = (InteractableHighlight)target;

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Highlight Validation", EditorStyles.boldLabel);

            if (!highlight.TryGetComponent<IInteractable>(out _))
            {
                EditorGUILayout.HelpBox("This InteractableHighlight is missing an Interactable!", MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("No issues found!", MessageType.Info);
            }
        }
    }
}
