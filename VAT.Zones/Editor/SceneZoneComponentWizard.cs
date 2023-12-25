using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;

namespace VAT.Zones.Editor
{
    public class SceneZoneComponentWizard : PopupWindowContent
    {
        private SceneZone[] _zones;

        private string _searchQuery = string.Empty;
        private Vector2 _scrollPosition;

        private List<Type> _componentTypes;

        public static void Initialize(SceneZone[] zones, Rect activatorRect)
        {
            var wizard = new SceneZoneComponentWizard
            {
                _zones = zones
            };

            wizard.LoadComponentTypes();

            PopupWindow.Show(activatorRect, wizard);
        }

        private void LoadComponentTypes()
        {
            _componentTypes = new List<Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    LoadComponentType(type);
                }
            }
        }

        private void LoadComponentType(Type type)
        {
            if (!type.IsAbstract && type.IsSubclassOf(typeof(ZoneComponent)))
            {
                _componentTypes.Add(type);
            }
        }

        private string CleanTypeName(string name)
        {
            return Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
        }

        public override void OnGUI(Rect rect)
        {
            _searchQuery = GUILayout.TextField(_searchQuery, EditorStyles.toolbarSearchField);

            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.FlexibleSpace();
                
                var style = EditorStyles.whiteLabel;

                if (string.IsNullOrWhiteSpace(_searchQuery))
                {
                    GUILayout.Label("Zone Component", style);
                }
                else
                {
                    GUILayout.Label("Search", style);
                }

                GUILayout.FlexibleSpace();
            }

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            var searchedTypes = _componentTypes
                .OrderByDescending(t => CleanTypeName(t.Name).Equals(_searchQuery))
                .Where(t => CleanTypeName(t.Name).ToLower().Contains(_searchQuery.ToLower()));

            foreach (var type in searchedTypes)
            {
                using (new GUILayout.HorizontalScope())
                {
                    var style = EditorStyles.toolbarButton;
                    style.alignment = TextAnchor.MiddleLeft;
                    style.padding.left = 20;

                    if (GUILayout.Button(CleanTypeName(type.Name), style))
                    {
                        foreach (var zone in _zones)
                        {
                            CreateZoneComponent(zone, type);
                        }

                        this.editorWindow.Close();
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        private void CreateZoneComponent(SceneZone zone, Type type)
        {
            var typeName = CleanTypeName(type.Name);
            var name = GameObjectUtility.GetUniqueNameForSibling(zone.transform, typeName);

            GameObject go = new(name, type);
            GameObjectUtility.SetParentAndAlign(go, zone.gameObject);

            EditorGUIUtility.PingObject(go);

            Undo.RegisterCreatedObjectUndo(go, $"Create {typeName}");
        }
    }
}
