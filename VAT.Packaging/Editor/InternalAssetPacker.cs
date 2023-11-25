using UnityEditor;
using UnityEngine;

using VAT.Cryst.Game;

using VAT.Serialization.JSON;

using VAT.Shared.Extensions;

namespace VAT.Packaging.Editor
{
    public static class InternalAssetPacker
    {
        /// <summary>
        /// Packs all internal text assets with default settings.
        /// </summary>
        [MenuItem("VAT/Internal/Pack Text Assets")]
        public static void PackTextAssets()
        {
            PackTextAssets(false);
        }

        /// <summary>
        /// Packs all internal text assets.
        /// </summary>
        /// <param name="isTemporary">Generates temporary addressables. Useful for fast pack times such as entering playmode.</param>
        public static void PackTextAssets(bool isTemporary)
        {
            // Refresh the asset packager
            AssetPackager.EditorForceRefresh();

            // Set the addressables info
            AddressablesManager.ClearGroups();
            InternalAddressablesManager.SetActiveSettings(!isTemporary);

            // Verify text asset folder
            var path = CrystAssetManager.GetProjectRelativePath(AssetPackager.CRYST_TEXT_ASSETS_FOLDER);
            if (AssetDatabase.IsValidFolder(path))
                AssetDatabase.DeleteAsset(path);

            AssetDatabase.CreateFolder(CrystAssetManager.PROJECT_RELATIVE_FOLDER, AssetPackager.CRYST_TEXT_ASSETS_FOLDER);

            // Save all packages as text assets
            foreach (var package in AssetPackager.Instance.GetPackages())
            {
                var packer = new JSONPacker();
                var json = packer.PackRoot(package);
                TextAsset textAsset = new(json.ToString());

                AssetDatabase.CreateAsset(textAsset, $"{path}/{package.Info.Title}.asset");

                textAsset.MarkAsAddressable(AssetPackager.INTERNAL_PACKAGES_GROUP, package.Address, AssetPackager.INTERNAL_PACKAGES_LABEL);
            }

            // Fix any potential issues
            if (!isTemporary)
            {
                AddressablesManager.FixGroups();
            }
        }
    }
}
