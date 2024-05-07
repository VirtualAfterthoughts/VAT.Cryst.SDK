using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

using UnityEditor;
using UnityEngine;

using VAT.Shared.Extensions;

namespace VAT.Packaging.Editor
{
    public class DataShardCreationWizard : EditorWindow
    {
        private Address _address = Address.EMPTY;
        private Crystal _crystal = null;
        private string _title = "My Data Shard";

        private Type _shardType;
        private string _shardDisplayName = string.Empty;

        public static void Initialize(Crystal crystal)
        {
            DataShardCreationWizard window = GetWindow<DataShardCreationWizard>(true, "Data Shard Creator");
            window._crystal = crystal;
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
            if (!type.IsAbstract && type.IsSubclassOf(typeof(DataShard)))
            {
                string displayName = type.Name;

                var attribute = type.GetCustomAttribute<DisplayNameAttribute>();

                if (attribute != null)
                {
                    displayName = attribute.DisplayName;
                }

                if (!header)
                {
                    menu.AddDisabledItem(new GUIContent($"{type.Assembly.GetName().Name} Data Shards"));
                    header = true;
                }

                menu.AddItem(new GUIContent(displayName), false, () =>
                {
                    _shardType = type;
                    _shardDisplayName = displayName;
                });
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

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Shard Type");

            if (GUILayout.Button(_shardDisplayName, EditorStyles.objectField))
            {
                var menu = new GenericMenu();
                LoadShardTypes(menu);
                menu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();

            // Recreate address
            string identifier = _shardDisplayName;

            _address = Address.BuildAddress(_crystal.CrystalInfo.Author, _crystal.CrystalInfo.Title, identifier, _title);

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
            if (GUILayout.Button("Create Shard", GUILayout.Width(200)))
            {
                InternalCreateShard();
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

            if (_shardType == null)
            {
                EditorGUILayout.LabelField("Missing Shard Type!", errorStyle);
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

        private void InternalCreateShard()
        {
            DataShard shard = ShardFactory.Create(_shardType) as DataShard;
            shard.ShardInfo = new ShardInfo()
            {
                Title = _title
            };
            shard.Address = _address;
            shard.AddressType = _shardDisplayName;

            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_crystal));
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
