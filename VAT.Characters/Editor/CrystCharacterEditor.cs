using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Characters.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(CrystCharacter), true)]
    public class CrystCharacterEditor : Editor
    {
        private bool _toggleRigs = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var character = target as CrystCharacter;

            if (character.Rigs != null && character.Rigs.Length > 0)
            {
                _toggleRigs = EditorGUILayout.BeginFoldoutHeaderGroup(_toggleRigs, "Rigs");

                if (_toggleRigs)
                {
                    EditorGUI.indentLevel = 1;

                    EditorGUI.BeginDisabledGroup(true);
                    foreach (var rig in character.Rigs)
                    {
                        EditorGUILayout.ObjectField($"Rig {rig.RigIndex}", rig, rig.GetType(), true);
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.indentLevel = 0;
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}
