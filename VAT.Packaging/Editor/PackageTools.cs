using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using VAT.Serialization.JSON;
using VAT.Shared.Extensions;

using VAT.Cryst.Game;

namespace VAT.Packaging.Editor
{
    public static class PackageTools
    {
        [MenuItem("VAT/Cryst SDK/Tools/Packages/Import Crystal")]
        public static void ImportCrystal()
        {
            string path = EditorUtility.OpenFilePanel("Import Crystal", Application.dataPath, "json");
            if (!string.IsNullOrWhiteSpace(path))
            {
                var json = path.ReadFromFile();
                JSONUnpacker unpacker = new(json);
                unpacker.UnpackRoot(out Crystal crystal, Crystal.Create);

                // Create package
                var assetsFolderPath = $"Assets/{CrystAssetManager.CRYST_ASSETS_FOLDER}";
                var packageFolderPath = $"{assetsFolderPath}/{AssetPackager.CRYST_CRYSTALS_FOLDER}";

                if (!AssetDatabase.IsValidFolder(packageFolderPath))
                {
                    AssetDatabase.CreateFolder(assetsFolderPath, AssetPackager.CRYST_CRYSTALS_FOLDER);
                }

                var addressPath = $"{packageFolderPath}/{crystal.Address}";
                if (!AssetDatabase.IsValidFolder(addressPath))
                {
                    AssetDatabase.CreateFolder(packageFolderPath, crystal.Address.ID);
                }

                AssetDatabase.CreateAsset(crystal, $"{addressPath}/{crystal.Info.Title}.asset");

                // Create shards
                var initialShards = crystal.Shards.ToArray();
                crystal.Shards.Clear();

                foreach (var shard in initialShards)
                {
                    AssetDatabase.CreateAsset(shard, $"{addressPath}/_{shard.Info.Title}.asset");
                    crystal.Shards.Add(shard);
                    shard.MainCrystal = crystal;

                    shard.ForceSerialize();
                }

                // Save
                crystal.ForceSerialize();

                // Show folder
                EditorUtility.RevealInFinder(addressPath);

                // Log
                Debug.Log($"AssetPackager -> Successfully imported {crystal.Address} into project!");
            }
        }

        public static void ExportPackage(Crystal crystal)
        {
            var packer = new JSONPacker();
            var json = packer.PackRoot(crystal);

            string path = EditorUtility.SaveFilePanel("Export Crystal", Application.dataPath, crystal.Address.ID, "json");
            if (!string.IsNullOrWhiteSpace(path))
            {
                json.WriteToFile(path);

                // Show file
                EditorUtility.RevealInFinder(path);

                // Log
                Debug.Log($"AssetPackager -> Successfully exported {crystal.Address} as JSON!");
            }
        }
    }
}
