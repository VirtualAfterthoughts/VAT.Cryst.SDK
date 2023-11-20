using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.AnalyzeRules;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.Assertions.Must;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace VAT.Packaging.Editor
{
    public static class AddressablesManager
    {
        private static AddressableAssetSettings _loadedSettings = null;
        public static AddressableAssetSettings LoadedSettings
        {
            get
            {
                if (_loadedSettings == null)
                {
                    var defaultSettings = AddressableAssetSettingsDefaultObject.GetSettings(true);
                    _loadedSettings = defaultSettings;
                }

                return _loadedSettings;
            }
        }

        public static void SetActiveSettings()
        {
            var settings = LoadedSettings;

            if (settings != null)
            {
                settings.activeProfileId = settings.profileSettings.GetProfileId("Default");
            }
        }

        public static void ClearGroups()
        {
            var settings = LoadedSettings;
            if (settings == null)
                return;

            foreach (var group in settings.groups.ToArray())
                settings.RemoveGroup(group);

            AssetDatabase.SaveAssets();
        }

        public static void ConfigureGroups(bool saveAssets)
        {
            var settings = LoadedSettings;
            if (settings == null)
                return;

            foreach (var group in settings.groups)
            {
                if (group == null)
                    continue;

                ConfigureGroup(group);
            }

            if (saveAssets)
            {
                AssetDatabase.SaveAssets();
            }
        }

        public static void ConfigureGroup(AddressableAssetGroup group)
        {
            group.RemoveSchema<PlayerDataGroupSchema>();

            var schema = group.GetSchema<BundledAssetGroupSchema>();
            if (!schema)
            {
                schema = group.AddSchema<BundledAssetGroupSchema>();
            }

            schema.IncludeInBuild = true;
            schema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            schema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }

        public static void FixGroups()
        {
            Debug.Log("FixGroups -> Configuring groups...");

            // Setup the base groups
            ConfigureGroups(false);

            // Fix rules
            DedupeAssets();

            // Save the new group data
            ConfigureGroups(true);
        }

        public static void DedupeAssets()
        {
            Debug.Log("DedupeAssets -> Checking...");

            var dedupe = new CheckBundleDupeDependencies();
            var analyzeResults = dedupe.RefreshAnalysis(LoadedSettings);

            int warnings = 0;

            foreach (var result in analyzeResults)
            {
                if (result.severity == MessageType.Warning)
                    warnings++;
            }

            if (warnings > 0)
            {
                Debug.Log($"DedupeAssets -> {warnings} issues found. Fixing...");

                dedupe.FixIssues(LoadedSettings);

                Debug.Log("DedupeAssets -> Done!");
            }
            else
            {
                Debug.Log("DedupeAssets -> No issues found!");
            }
        }
    }
}
