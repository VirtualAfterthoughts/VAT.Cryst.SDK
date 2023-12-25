using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Zones.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(SceneZone))]
    [CanEditMultipleObjects]
    public class SceneZoneEditor : Editor
    {
        private SerializedProperty _zoneCollider;
        private SerializedProperty _adjacentZones;
        private SerializedProperty _zoneComponents;
        private SerializedProperty _entityMask;

        private void OnEnable()
        {
            _zoneCollider = serializedObject.FindProperty("_zoneCollider");
            _adjacentZones = serializedObject.FindProperty("_adjacentZones");
            _zoneComponents = serializedObject.FindProperty("_zoneComponents");
            _entityMask = serializedObject.FindProperty("_entityMask");
        }

        private Rect _buttonRect;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SceneZone[] zones = new SceneZone[targets.Length];

            for (var i = 0; i < targets.Length; i++)
                zones[i] = targets[i] as SceneZone;

            // Locked information
            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(_zoneCollider);
            EditorGUILayout.PropertyField(_adjacentZones);
            EditorGUILayout.PropertyField(_zoneComponents);

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(_entityMask);

            // Buttons
            GUILayout.Space(5);

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add Zone Component", GUILayout.Width(225), GUILayout.Height(25)))
            {
                SceneZoneComponentWizard.Initialize(zones, _buttonRect);
            }

            if (Event.current.type == EventType.Repaint) 
            { 
                _buttonRect = GUILayoutUtility.GetLastRect(); 
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

            // Apply changes
            serializedObject.ApplyModifiedProperties();
        }
    }
}