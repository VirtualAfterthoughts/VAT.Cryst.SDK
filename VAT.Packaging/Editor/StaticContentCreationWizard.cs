using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

using VAT.Shared.Extensions;

using Object = UnityEngine.Object;

namespace VAT.Packaging.Editor
{
    public class StaticContentCreationWizard : EditorWindow
    {
        private Address _address = Address.EMPTY;
        private Package _package = null;
        private string _title = "My Content";
        private Object _mainAsset = null;

        private Type _contentType;
        private StaticContentIdentifierAttribute _contentIdentifier;

        public static void Initialize(Package package)
        {
            StaticContentCreationWizard window = GetWindow<StaticContentCreationWizard>(true, "Content Creator");
            window._package = package;
            window.Show();
        }

        private void LoadContentTypes(GenericMenu menu)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                bool header = false;

                foreach (Type type in assembly.GetTypes())
                {
                    LoadContentType(type, menu, ref header);
                }
            }
        }

        private void LoadContentType(Type type, GenericMenu menu, ref bool header)
        {
            if (!type.IsAbstract && type.IsSubclassOf(typeof(StaticContent)))
            {
                var attribute = type.GetCustomAttribute<StaticContentIdentifierAttribute>();

                if (attribute != null)
                {
                    if (!header)
                    {
                        menu.AddDisabledItem(new GUIContent($"{type.Assembly.GetName().Name} Contents"));
                        header = true;
                    }

                    menu.AddItem(new GUIContent(attribute.displayName), false, () =>
                    {
                        _contentType = type;
                        _contentIdentifier = attribute;
                    });
                }
            }
        }

        public void OnGUI()
        {
            // Header
            EditorGUILayout.LabelField("Content Settings", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            // Spacing
            GUILayout.Space(5);

            // Draw options
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Address", _address);
            EditorGUILayout.ObjectField("Package", _package, typeof(Package), false);
            EditorGUI.EndDisabledGroup();

            _title = EditorGUILayout.TextField("Content Title", _title);

            EditorGUI.BeginChangeCheck();

            Type objectType = _contentIdentifier != null ? _contentIdentifier.mainAssetType : typeof(Object);

            _mainAsset = EditorGUILayout.ObjectField("Main Asset", _mainAsset, objectType, false);

            if (EditorGUI.EndChangeCheck() && _mainAsset != null)
            {
                _title = _mainAsset.name;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Content Type");

            if (GUILayout.Button(_contentIdentifier != null ? _contentIdentifier.displayName : "", EditorStyles.objectField))
            {
                var menu = new GenericMenu();
                LoadContentTypes(menu);
                menu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();

            // Recreate address
            string identifier = _contentIdentifier?.displayName ?? "Unknown";

            _address = Address.BuildAddress(_package.PackageInfo.Author, _package.PackageInfo.Title, identifier, _title);

            // Verify content creation
            if (!InternalValidateContentSettings())
                return;

            // Spacing
            GUILayout.Space(5);

            // Header
            EditorGUILayout.LabelField("Options", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            // Spacing
            GUILayout.Space(5);

            // Allow content creation
            if (GUILayout.Button("Create Content", GUILayout.Width(200)))
            {
                InternalCreateContent();
                Close();
            }
        }

        private bool InternalValidateContentSettings()
        {
            GUILayout.Space(5);

            // Header
            EditorGUILayout.LabelField("Errors", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            bool isValid = true;

            var errorStyle = new GUIStyle(EditorStyles.boldLabel);
            errorStyle.normal.textColor = Color.red;

            if (_mainAsset == null)
            {
                EditorGUILayout.LabelField("Missing Main Asset!", errorStyle);
                isValid = false;
            }
            else if (_contentType == null)
            {
                EditorGUILayout.LabelField("Missing Content Type!", errorStyle);
                isValid = false;
            }
            else if (!_contentIdentifier.mainAssetType.IsAssignableFrom(_mainAsset.GetType()))
            {
                EditorGUILayout.LabelField($"Main Asset is not a {_contentIdentifier.mainAssetType.Name}!", errorStyle);
                isValid = false;
            }
            else if (AssetPackager.Instance.HasContent(_address))
            {
                EditorGUILayout.HelpBox("There's already content at that address!", MessageType.Error);
                isValid = false;
            }

            if (isValid)
            {
                EditorGUILayout.LabelField("No issues found!");
            }

            return isValid;
        }

        private void InternalCreateContent()
        {
            StaticContent content = ContentFactory.Create(_contentType) as StaticContent;
            content.ContentInfo = new ContentInfo()
            {
                Title = _title
            };
            content.MainPackage = _package;
            content.Address = _address;
            content.AddressType = _contentIdentifier?.displayName ?? "Unknown";
            content.SetAsset(_mainAsset);

            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_package));
            var fileName = $"{path}/_{_title}";
            var fileExtension = ".asset";

            var filePath = $"{fileName}{fileExtension}";
            int suffix = 0;

            // Find a unique name for the file
            while (AssetDatabase.LoadAllAssetsAtPath(filePath).Length > 0)
            {
                filePath = $"{fileName}_{suffix++}{fileExtension}";

                // Terminate incase we ever reach here somehow
                if (suffix > 1000)
                {
                    Debug.LogError("Terminating content creation, too many files with the same name!");
                    return;
                }
            }

            AssetDatabase.CreateAsset(content, filePath);
            _package.Contents.Add(content);

            _package.OnValidate();

            content.ForceSerialize();
            _package.ForceSerialize();

            AssetPackager.EditorForceRefresh();

            // Show file in editor
            Selection.SetActiveObjectWithContext(content, content);

        }
    }
}
