using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Props.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(GunHammer), true)]
    public class GunHammerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var hammer = (GunHammer)target;

            // Draw read only states
            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.EnumFlagsField("State", hammer.State);

            EditorGUI.EndDisabledGroup();

            // Draw actuation buttons
            if (GUILayout.Button("Cock"))
            {
                hammer.Cock();
            }

            if (GUILayout.Button("Release"))
            {
                hammer.Release();
            }
        }
    }
}
