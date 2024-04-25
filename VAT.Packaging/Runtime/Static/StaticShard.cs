using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

using VAT.Cryst.Game;
using VAT.Serialization.JSON;
using VAT.Shared.Extensions;

using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    public abstract class StaticShard : Shard
    {
        protected Crystal _crystal;
        public Crystal StaticCrystal 
        { 
            get 
            {
#if UNITY_EDITOR
                if (!Application.isPlaying && _crystal == null)
                {
                    ValidateCrystal();
                }
#endif

                return _crystal; 
            } 
            set 
            { 
                _crystal = value; 
            } 
        }

        public override ICrystal MainCrystal { get => StaticCrystal; set => StaticCrystal = value as Crystal; }

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
            var packageInfo = StaticCrystal.CrystalInfo;

            if (!string.IsNullOrWhiteSpace(AddressType))
            {
                Address = Address.BuildAddress(packageInfo.Author, packageInfo.Title, AddressType, ShardInfo.Title);
            }
            else
            {
                Address = Address.BuildAddress(packageInfo.Author, packageInfo.Title, ShardInfo.Title);
            }
        }

        public virtual List<StaticPackedAsset> CollectPackedAssets()
        {
            return new List<StaticPackedAsset>();
        }

        public virtual void GeneratePackedAssets(bool isBuilding = false) { }

        protected virtual void OnUnpackPackedAssets(List<StaticPackedAsset> packedAssets) { }

        protected override void OnPack(JSONPacker packer, JObject json)
        {
#if UNITY_EDITOR
            GeneratePackedAssets(true);
            ValidateAssets(true);
#endif

            if (StaticAsset != null)
            {
                json.Add("mainAsset", StaticAsset.AssetGUID);
            }

            if (_crystal != null)
            {
                json.Add("package", packer.PackReference(_crystal));
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
                unpacker.TryCreateFromReference(package, out _crystal, Crystal.Create);
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
                return $"{Address.CleanAddress(MainCrystal.Info.Title)} {EditorAssetGroup}";
            }
        }

        public void OnValidate()
        {
            // Save asset info
            ValidateAssets(false);
        }

        public void ValidateAssets(bool isBuilding = false)
        {
            ValidateCrystal();

            ValidateAsset(StaticAsset, Address, isBuilding);

            OnValidateAssets(isBuilding);
        }

        protected void ValidateCrystal()
        {
            var path = AssetDatabase.GetAssetPath(this);

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            var projectPath = CrystAssetManager.GetProjectPath();
            string folder = Path.GetDirectoryName(projectPath + "/" + path);

            string[] files = Directory.GetFiles(folder);

            foreach (var file in files)
            {
                if (!file.EndsWith(".asset"))
                    continue;

                string final = file.Replace(projectPath, "");

                var crystal = AssetDatabase.LoadAssetAtPath<Crystal>(final);
                if (crystal != null)
                {
                    StaticCrystal = crystal;
                    break;
                }
            }
        }

        protected void ValidateAsset(StaticCrystAsset asset, Address address, bool isBuilding = false)
        {
            asset.ValidateGUID();

            var editorAsset = asset.EditorAsset;
            if (isBuilding && editorAsset && !editorAsset.IsAddressable())
            {
                editorAsset.MarkAsAddressable(AddressableGroupName, address, null);
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

    public abstract class StaticShardT<T> : StaticShard, IShardT<T> where T : Object
    {
        public IWeakAssetT<T> MainAssetT => MainAsset as IWeakAssetT<T>;

#if UNITY_EDITOR
        public override string EditorAssetGroup => typeof(T).Name;

        public override Type EditorAssetType => typeof(T);
#endif
    }
}
