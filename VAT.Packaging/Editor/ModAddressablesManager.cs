using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

using UnityEngine;

namespace VAT.Packaging.Editor
{
    public static class ModAddressablesManager
    {
        public const string BuildPrefix = "BuiltCrystals";

        private struct ProfileVariables
        {
            public const string ProfileName = "Mods";
            public const string LocalBuildPath = "Local.BuildPath";
        }

        private struct ProfileValues
        {
            public const string LocalBuildPath = "[UnityEngine.AddressableAssets.Addressables.BuildPath]";
        }

        public static void SetActiveSettings()
        {
            var settings = AddressablesManager.LoadedSettings;
            if (settings != null)
            {
                string id = settings.profileSettings.AddProfile(ProfileVariables.ProfileName, "Default");

                settings.activeProfileId = id;
                settings.profileSettings.SetValue(id, ProfileVariables.LocalBuildPath, ProfileValues.LocalBuildPath);

                AssetDatabase.SaveAssets();
            }
        }

        public static string GetRootFolder()
        {
            string folderPath = $"{BuildPrefix}/{EditorUserBuildSettings.activeBuildTarget}";
            string parent = Directory.GetParent(Application.dataPath).FullName;
            return $"{parent}/{folderPath}";
        }

        public static string GetBuildPath(Crystal crystal)
        {
            string folderPath = $"{BuildPrefix}/{EditorUserBuildSettings.activeBuildTarget}/{crystal.Address}";
            string parent = Directory.GetParent(Application.dataPath).FullName;
            string fullPath = $"{parent}/{folderPath}";

            if (Directory.Exists(fullPath))
                Directory.Delete(fullPath, true);

            Directory.CreateDirectory(fullPath);

            return fullPath;
        }
    }
}
