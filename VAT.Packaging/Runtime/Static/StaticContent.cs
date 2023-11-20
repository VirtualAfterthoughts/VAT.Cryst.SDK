using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using VAT.Serialization.JSON;
using VAT.Shared.Extensions;

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

        protected override void OnPack(JSONPacker packer, JObject json)
        {
#if UNITY_EDITOR
            ValidateAsset(true);
#endif

            if (StaticAsset != null)
            {
                json.Add("mainAsset", StaticAsset.AssetGUID);
            }

            if (_package != null)
            {
                json.Add("package", packer.PackReference(_package));
            }

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

            base.OnUnpack(unpacker, json);
        }

#if UNITY_EDITOR
        public virtual string Group => string.Empty;

        public void OnValidate()
        {
            // Save asset info
            ValidateAsset(false);
        }

        public void ValidateAsset(bool isBuilding = false)
        {
            StaticAsset.ValidateGUID();

            if (isBuilding && MainAsset != null && StaticAsset.EditorAsset && MainPackage != null)
            {
                var groupName = $"{Address.CleanAddress(MainPackage.Info.Title)} {Group}";

                StaticAsset.EditorAsset.MarkAsAddressable(groupName, Address, null);
            }
        }

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
    }
}
