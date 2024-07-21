using UnityEditor;
using UnityEngine;

using VAT.Cryst.Addressables;
using VAT.Cryst.Game;

using VAT.Serialization.JSON;

namespace VAT.Packaging.Editor
{
    public static class InternalAssetPacker
    {
        /// <summary>
        /// Packs all internal text assets with default settings.
        /// </summary>
        [MenuItem("Virtual Afterthoughts/Debug/Pack Game Assets")]
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
            var path = CrystAssetManager.GetCrystRelativePath(AssetPackager.CRYST_TEXT_ASSETS_FOLDER);
            if (AssetDatabase.IsValidFolder(path))
                AssetDatabase.DeleteAsset(path);

            AssetDatabase.CreateFolder(CrystAssetManager.PROJECT_RELATIVE_FOLDER, AssetPackager.CRYST_TEXT_ASSETS_FOLDER);

            // Save all crystals as text assets
            foreach (var crystal in AssetPackager.Instance.GetCrystals())
            {
                var packer = new JSONPacker();
                var json = packer.PackRoot(crystal);
                TextAsset textAsset = new(json.ToString());

                AssetDatabase.CreateAsset(textAsset, $"{path}/{crystal.Info.Title}.asset");

                var group = AddressablesExtensions.CreateOrFindGroup(AssetPackager.INTERNAL_CRYSTALS_GROUP);

                var entry = textAsset.SetAddressable(group);

                entry.SetAddress(crystal.Address.ID);
                entry.SetLabel(AssetPackager.INTERNAL_CRYSTALS_LABEL, true);
            }

            // Fix any potential issues
            if (!isTemporary)
            {
                AddressablesManager.FixGroups();
            }
        }
    }
}
