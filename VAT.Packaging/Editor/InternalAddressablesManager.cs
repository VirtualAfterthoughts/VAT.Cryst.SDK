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
    public static class InternalAddressablesManager
    {
        private struct ProfileVariables
        {
            public const string ProfileName = "Internal";
        }

        public static void SetActiveSettings()
        {
            SetActiveSettings(true);
        }

        public static void SetActiveSettings(bool saveAssets)
        {
            var settings = AddressablesManager.LoadedSettings;
            if (settings != null)
            {
                string id = settings.profileSettings.AddProfile(ProfileVariables.ProfileName, "Default");
                settings.activeProfileId = id;

                if (saveAssets)
                {
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }
}
