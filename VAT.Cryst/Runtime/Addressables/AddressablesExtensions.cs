using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

using UnityEngine;

namespace VAT.Cryst.Addressables
{
    public static class AddressablesExtensions
    {
#if UNITY_EDITOR
        public static bool IsAddressable(this Object asset)
        {
            return IsAddressable(asset, AddressableAssetSettingsDefaultObject.Settings);
        }

        public static bool IsAddressable(this Object asset, AddressableAssetSettings settings)
        {
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out long _))
            {
                var entry = settings.FindAssetEntry(guid);
                return entry != null;
            }

            return false;
        }

        public static AddressableAssetEntry GetAssetEntry(this Object asset)
        {
            return GetAssetEntry(asset, AddressableAssetSettingsDefaultObject.Settings);
        }

        public static AddressableAssetEntry GetAssetEntry(this Object asset, AddressableAssetSettings settings)
        {
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out long _))
            {
                var entry = settings.FindAssetEntry(guid);
                return entry;
            }

            return null;
        }

        public static AddressableAssetEntry SetAddressable(this Object asset, AddressableAssetGroup group)
        {
            var settings = group.Settings;

            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out long _))
            {
                return null;
            }

            return settings.CreateOrMoveEntry(guid, group);
        }

        public static AddressableAssetGroup CreateOrFindGroup(string groupName)
        {
            return CreateOrFindGroup(groupName, AddressableAssetSettingsDefaultObject.Settings);
        }

        public static AddressableAssetGroup CreateOrFindGroup(string groupName, AddressableAssetSettings settings)
        {
            var found = settings.FindGroup(groupName);

            if (found != null)
            {
                return found;
            }

            return settings.CreateGroup(groupName, false, false, false, new List<AddressableAssetGroupSchema>());
        }
#endif
    }
}
