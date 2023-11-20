using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;

using VAT.Serialization.JSON;

namespace VAT.Packaging.Editor {
    public static class ExternalAssetPacker {
        public static void PackPackages(BuildTarget target = BuildTarget.StandaloneWindows64, bool revealFolder = true) {
            if (!AssetPackager.IsReady) {
                Debug.LogWarning("Cannot pack packages because AssetPackager is not ready!");
                return;
            }

            foreach (var package in AssetPackager.Instance.GetPackages()) {
                PackPackage(package, target, false);
            }

            if (revealFolder) {
                string path = ModAddressablesManager.GetRootFolder();
                EditorUtility.RevealInFinder(path);
                Debug.Log("AssetPackager -> Successfully built all packages!");
            }
        }

        public static void PackPackage(Package package, BuildTarget target = BuildTarget.StandaloneWindows64, bool revealFolder = true) {
            // Set the build target
            var activeTarget = EditorUserBuildSettings.activeBuildTarget;
            var activeGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            var newGroup = BuildTargetGroup.Standalone;
            if (target == BuildTarget.Android)
                newGroup = BuildTargetGroup.Android;

            EditorUserBuildSettings.SwitchActiveBuildTarget(newGroup, target);

            // Setup the addressable settings
            ModAddressablesManager.SetActiveSettings();
            AddressablesManager.ClearGroups();

            // Pack the json, which will also create the addressable groups
            var jsonPacker = new JSONPacker();
            var json = jsonPacker.PackRoot(package);

            // Build the addressable content
            AddressablesManager.FixGroups();

            Internal_BuildPackage();

            // Copy addressables to desired folder
            string buildPath = ModAddressablesManager.GetBuildPath(package);
            Internal_CopyBuildToFolder(buildPath);

            // Write the package into a json file
            json.WriteToFile($"{buildPath}/{Package.BUILT_NAME}");

            // Reset the build settings
            AddressablesManager.SetActiveSettings();
            EditorUserBuildSettings.SwitchActiveBuildTarget(activeGroup, activeTarget);

            // Show build folder
            if (revealFolder) {
                EditorUtility.RevealInFinder(buildPath + "/");
                Debug.Log($"AssetPackager -> Successfully built {package.Address}!");
            }
        }

        private static void Internal_BuildPackage() {
            AddressablesRuntimeProperties.ClearCachedPropertyValues();
            AddressableAssetSettings.BuildPlayerContent();
        }

        private static void Internal_CopyBuildToFolder(string path) {
            string buildPath = Addressables.BuildPath;

            // Copy all directories
            foreach (string dirPath in Directory.GetDirectories(buildPath, "*", SearchOption.AllDirectories)) {
                Directory.CreateDirectory(dirPath.Replace(buildPath, path));
            }

            // Copy all files
            foreach (string newPath in Directory.GetFiles(buildPath, "*.*", SearchOption.AllDirectories)) {
                File.Copy(newPath, newPath.Replace(buildPath, path), true);
            }
        }
    }
}
