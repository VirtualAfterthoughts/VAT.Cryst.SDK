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
    public class StaticShardCreationWizard : EditorWindow
    {
        private Address _address = Address.EMPTY;
        private Crystal _crystal = null;
        private string _title = "My Shard";
        private Object _mainAsset = null;

        private Type _contentType;
        private StaticShardIdentifier _contentIdentifier;

        public static void Initialize(Crystal crystal)
        {
            StaticShardCreationWizard window = GetWindow<StaticShardCreationWizard>(true, "Shard Creator");
            window._crystal = crystal;
            window.Show();
        }

        public static void Initialize(Crystal crystal, StaticShardIdentifier identifier, Type contentType, Object mainAsset)
        {
            StaticShardCreationWizard window = GetWindow<StaticShardCreationWizard>(true, "Shard Creator");
            window._crystal = crystal;
            window._mainAsset = mainAsset;
            window._title = mainAsset.name;
            window._contentType = contentType;
            window._contentIdentifier = identifier;
            window.Show();
        }

        private void LoadShardTypes(GenericMenu menu)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                bool header = false;

                foreach (Type type in assembly.GetTypes())
                {
                    LoadShardType(type, menu, ref header);
                }
            }
        }

        private void LoadShardType(Type type, GenericMenu menu, ref bool header)
        {
            if (!type.IsAbstract && type.IsSubclassOf(typeof(StaticShard)))
            {
                var attribute = type.GetCustomAttribute<StaticShardIdentifier>();

                if (attribute != null)
                {
                    if (!header)
                    {
                        menu.AddDisabledItem(new GUIContent($"{type.Assembly.GetName().Name} Shards"));
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
            EditorGUILayout.LabelField("Shard Settings", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            // Spacing
            GUILayout.Space(5);

            // Draw options
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Address", _address);
            EditorGUILayout.ObjectField("Crystal", _crystal, typeof(Crystal), false);
            EditorGUI.EndDisabledGroup();

            _title = EditorGUILayout.TextField("Shard Title", _title);

            EditorGUI.BeginChangeCheck();

            Type objectType = _contentIdentifier != null ? _contentIdentifier.mainAssetType : typeof(Object);

            _mainAsset = EditorGUILayout.ObjectField("Main Asset", _mainAsset, objectType, false);

            if (EditorGUI.EndChangeCheck() && _mainAsset != null)
            {
                _title = _mainAsset.name;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Shard Type");

            if (GUILayout.Button(_contentIdentifier != null ? _contentIdentifier.displayName : "", EditorStyles.objectField))
            {
                var menu = new GenericMenu();
                LoadShardTypes(menu);
                menu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();

            // Recreate address
            string identifier = _contentIdentifier?.displayName ?? "Unknown";

            _address = Address.BuildAddress(_crystal.CrystalInfo.Author, _crystal.CrystalInfo.Title, identifier, _title);

            // Verify shard creation
            if (!InternalValidateContentSettings())
                return;

            // Spacing
            GUILayout.Space(5);

            // Header
            EditorGUILayout.LabelField("Options", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            // Spacing
            GUILayout.Space(5);

            // Allow shard creation
            if (GUILayout.Button("Create Shard", GUILayout.Width(200)))
            {
                CreateShard();
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
                EditorGUILayout.LabelField("Missing Shard Type!", errorStyle);
                isValid = false;
            }
            else if (!_contentIdentifier.mainAssetType.IsAssignableFrom(_mainAsset.GetType()))
            {
                EditorGUILayout.LabelField($"Main Asset is not a {_contentIdentifier.mainAssetType.Name}!", errorStyle);
                isValid = false;
            }
            else if (AssetPackager.Instance.HasShard(_address))
            {
                EditorGUILayout.HelpBox("There's already a shard at that address!", MessageType.Error);
                isValid = false;
            }

            if (isValid)
            {
                EditorGUILayout.LabelField("No issues found!");
            }

            return isValid;
        }

        public static void CreateDefaultShard(Type type, StaticShardIdentifier identifier, Crystal crystal, Object mainAsset)
        {
            StaticShard shard = ShardFactory.Create(type) as StaticShard;
            string title = mainAsset.name;
            shard.ShardInfo = new ShardInfo()
            {
                Title = title
            };
            shard.StaticCrystal = crystal;
            shard.AddressType = identifier?.displayName ?? "Unknown";
            shard.BuildAddress();
            shard.SetAsset(mainAsset);

            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(crystal));
            var fileName = $"{path}/_{title}";
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
                    Debug.LogError("Terminating shard creation, too many files with the same name!");
                    return;
                }
            }

            AssetDatabase.CreateAsset(shard, filePath);
            crystal.Shards.Add(shard);

            crystal.OnValidate();

            shard.ForceSerialize();
            crystal.ForceSerialize();

            AssetPackager.EditorForceRefresh();
        }

        public void CreateShard()
        {
            StaticShard shard = ShardFactory.Create(_contentType) as StaticShard;
            shard.ShardInfo = new ShardInfo()
            {
                Title = _title
            };
            shard.StaticCrystal = _crystal;
            shard.Address = _address;
            shard.AddressType = _contentIdentifier?.displayName ?? "Unknown";
            shard.SetAsset(_mainAsset);

            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_crystal));
            var fileName = $"{path}/{_title}";
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
                    Debug.LogError("Terminating shard creation, too many files with the same name!");
                    return;
                }
            }

            AssetDatabase.CreateAsset(shard, filePath);
            _crystal.Shards.Add(shard);

            _crystal.OnValidate();

            shard.ForceSerialize();
            _crystal.ForceSerialize();

            AssetPackager.EditorForceRefresh();

            // Show file in editor
            Selection.SetActiveObjectWithContext(shard, shard);
        }
    }
}
