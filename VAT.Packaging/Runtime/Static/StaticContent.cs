using Newtonsoft.Json.Linq;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using VAT.Serialization.JSON;
using VAT.Shared.Extensions;

using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    public abstract class StaticContent : Content
    {
        [SerializeField]
        protected Package _package;
        public Package StaticPackage { get { return _package; } set { _package = value; } }

        public override IPackage MainPackage { get => StaticPackage; set => StaticPackage = value as Package; }

        public abstract StaticCrystAsset StaticAsset { get; set; }

        public override IWeakAsset MainAsset { get => StaticAsset; }

        [SerializeField]
        private string _addressType;
        public virtual string AddressType
        {
            get
            {
                return _addressType;
            }
            set
            {
                _addressType = value;
            }
        }

        public override void BuildAddress()
        {
            var packageInfo = StaticPackage.PackageInfo;

            if (!string.IsNullOrWhiteSpace(AddressType))
            {
                Address = Address.BuildAddress(packageInfo.Author, packageInfo.Title, AddressType, ContentInfo.Title);
            }
            else
            {
                Address = Address.BuildAddress(packageInfo.Author, packageInfo.Title, ContentInfo.Title);
            }
        }

        public virtual List<StaticPackedAsset> CollectPackedAssets()
        {
            return new List<StaticPackedAsset>();
        }

        protected virtual void OnUnpackPackedAssets(List<StaticPackedAsset> packedAssets) { }

        protected override void OnPack(JSONPacker packer, JObject json)
        {
#if UNITY_EDITOR
            ValidateAssets(true);
#endif

            if (StaticAsset != null)
            {
                json.Add("mainAsset", StaticAsset.AssetGUID);
            }

            if (_package != null)
            {
                json.Add("package", packer.PackReference(_package));
            }

            var packedAssets = CollectPackedAssets();
            var packedAssetJArray = new JArray();

            foreach (var asset in packedAssets)
            {
                JObject assetJObject = new();
                asset.Pack(packer, assetJObject);
                packedAssetJArray.Add(assetJObject);
            }

            json.Add("packedAssets", packedAssetJArray);

            base.OnPack(packer, json);
        }

        protected override void OnUnpack(JSONUnpacker unpacker, JObject json)
        {
            if (json.TryGetValue("mainAsset", out var mainAsset))
            {
                StaticAsset = new StaticCrystAsset(mainAsset.ToString());
            }

            if (json.TryGetValue("package", out var package))
            {
                unpacker.TryCreateFromReference(package, out _package, Package.Create);
            }

            if (json.TryGetValue("packedAssets", out var packedAssets))
            {
                List<StaticPackedAsset> assetList = new();

                foreach (var packedAsset in (JArray)packedAssets)
                {
                    var asset = new StaticPackedAsset();
                    asset.Unpack(unpacker, packedAsset);

                    assetList.Add(asset);
                }

                OnUnpackPackedAssets(assetList);
            }

            base.OnUnpack(unpacker, json);
        }

#if UNITY_EDITOR
        public virtual string EditorAssetGroup => string.Empty;

        public virtual Type EditorAssetType => typeof(Object);

        public virtual string AddressableGroupName
        {
            get
            {
                return $"{Address.CleanAddress(MainPackage.Info.Title)} {EditorAssetGroup}";
            }
        }

        public void OnValidate()
        {
            // Save asset info
            ValidateAssets(false);
        }

        public void ValidateAssets(bool isBuilding = false)
        {
            ValidateAsset(StaticAsset, Address, isBuilding);

            OnValidateAssets(isBuilding);
        }

        protected void ValidateAsset(StaticCrystAsset asset, Address address, bool isBuilding = false)
        {
            asset.ValidateGUID();

            if (isBuilding && asset != null && asset.EditorAsset && MainPackage != null)
            {
                asset.EditorAsset.MarkAsAddressable(AddressableGroupName, address, null);
            }
        }

        protected virtual void OnValidateAssets(bool isBuilding = false) { }

        public void SetAsset(Object asset)
        {
            if (asset == null)
                StaticAsset = new StaticCrystAsset(null);
            else if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long _))
                StaticAsset = new StaticCrystAsset(guid);
            else
            {
                Debug.LogError("Failed to find GUID of target asset.");
                return;
            }

            StaticAsset.ValidateGUID(asset);
        }

        public void SetAsset(string guid)
        {
            StaticAsset = new StaticCrystAsset(guid);
            StaticAsset.ValidateGUID();
        }
#endif
    }

    public abstract class StaticContentT<T> : StaticContent, IContentT<T> where T : Object
    {
        public IWeakAssetT<T> MainAssetT => MainAsset as IWeakAssetT<T>;

#if UNITY_EDITOR
        public override string EditorAssetGroup => typeof(T).Name;

        public override Type EditorAssetType => typeof(T);
#endif
    }
}
