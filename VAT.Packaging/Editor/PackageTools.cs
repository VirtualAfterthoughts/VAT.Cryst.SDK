using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using VAT.Serialization.JSON;
using VAT.Shared.Extensions;

using VAT.Cryst;

namespace VAT.Packaging.Editor {
    public static class PackageTools {
        [MenuItem("VAT/Cryst SDK/Tools/Packages/Import Package")]
        public static void ImportPackage() {
            string path = EditorUtility.OpenFilePanel("Import Package", Application.dataPath, "json");
            if (!string.IsNullOrWhiteSpace(path)) {
                var json = path.ReadFromFile();
                JSONUnpacker unpacker = new (json);
                unpacker.UnpackRoot(out Package package, Package.Create);

                // Create package
                var assetsFolderPath = $"Assets/{CrystAssetManager.CRYST_ASSETS_FOLDER}";
                var packageFolderPath = $"{assetsFolderPath}/{AssetPackager.CRYST_PACKAGES_FOLDER}";

                if (!AssetDatabase.IsValidFolder(packageFolderPath)) {
                    AssetDatabase.CreateFolder(assetsFolderPath, AssetPackager.CRYST_PACKAGES_FOLDER);
                }

                var addressPath = $"{packageFolderPath}/{package.Address}";
                if (!AssetDatabase.IsValidFolder(addressPath)) {
                    AssetDatabase.CreateFolder(packageFolderPath, package.Address);
                }

                AssetDatabase.CreateAsset(package, $"{addressPath}/{package.Info.Title}.asset");

                // Create contents
                var initialContents = package.Contents.ToArray();
                package.Contents.Clear();

                foreach (var content in initialContents) {
                    AssetDatabase.CreateAsset(content, $"{addressPath}/_{content.Info.Title}.asset");
                    package.Contents.Add(content);
                    content.MainPackage = package;

                    content.ForceSerialize();
                }

                // Save
                package.ForceSerialize();

                // Show folder
                EditorUtility.RevealInFinder(addressPath);

                // Log
                Debug.Log($"AssetPackager -> Successfully imported {package.Address} into project!");
            }
        }

        public static void ExportPackage(Package package) {
            var packer = new JSONPacker();
            var json = packer.PackRoot(package);

            string path = EditorUtility.SaveFilePanel("Export Package", Application.dataPath, package.Address, "json");
            if (!string.IsNullOrWhiteSpace(path)) {
                json.WriteToFile(path);

                // Show file
                EditorUtility.RevealInFinder(path);

                // Log
                Debug.Log($"AssetPackager -> Successfully exported {package.Address} as JSON!");
            }
        }
    }
}
