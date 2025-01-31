using System;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Cryst.Game;

namespace VAT.Cryst.Editor
{
    using System.ComponentModel;
    using System.Reflection;
    using UnityEditor;

    [CustomEditor(typeof(CrystSettings))]
    public class CrystSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add Sub Settings"))
            {
                AddMenu();
            }
        }

        private void AddMenu()
        {
            var parent = target as CrystSettings;

            GenericMenu menu = new();
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<SubCrystSettings>();
            foreach (Type type in types)
            {
                if (parent.GetSettings(type) != null)
                {
                    continue;
                }

                var displayName = GetMenuNameFromType(type);
                menu.AddItem(new GUIContent(displayName), false, AddSubSettings, type.Name);
            }
            menu.ShowAsContext();
        }

        private string GetMenuNameFromType(Type type)
        {
            var attribute = type.GetCustomAttribute<DisplayNameAttribute>();

            if (attribute != null)
            {
                return attribute.DisplayName;
            }

            return type.Name;
        }

        private void AddSubSettings(object type)
        {
            SubCrystSettings settings = CreateInstance((string)type) as SubCrystSettings;
            settings.name = (string)type;

            var path = AssetDatabase.GetAssetPath(target);
            AssetDatabase.AddObjectToAsset(settings, path);

            var parent = target as CrystSettings;
            parent.AddSettings(settings);

            Selection.activeObject = settings;
        }
    }
}