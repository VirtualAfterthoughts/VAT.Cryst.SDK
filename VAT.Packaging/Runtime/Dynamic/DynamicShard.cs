using Newtonsoft.Json.Linq;
using System;

using UnityEngine;
using VAT.Serialization.JSON;
using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    public static class DynamicShardFactory
    {
        public static T Create<T>(ShardInfo info, Object asset) where T : DynamicShard
        {
            var shard = ScriptableObject.CreateInstance<T>();
            shard.name = info.Title;
            shard.ShardInfo = info;
            shard.DynamicAsset = new DynamicCrystAsset(asset);

            shard.BuildAddress();

            AssetPackager.HookOnReady(() =>
            {
                AssetPackager.Instance.LoadShard(shard);
            });

            return shard;
        }
    }

    public abstract class DynamicShard : AssetShard
    {
        private DynamicCrystAsset _mainAsset;
        public virtual DynamicCrystAsset DynamicAsset
        {
            get
            {
                return _mainAsset;
            }
            set
            {
                _mainAsset = value;
            }
        }

        public override IWeakAsset MainAsset => DynamicAsset;

        public override ICrystal MainCrystal
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void BuildAddress()
        {
            Address = new(Address.BuildAddress("Runtime", "Generated", Info.Title));
        }

        protected override void OnPack(JSONPacker packer, JObject json)
        {
            throw new NotImplementedException();
        }

        protected override void OnUnpack(JSONUnpacker unpacker, JObject json)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class DynamicShardT<T> : DynamicShard, IAssetShardT<T> where T : Object
    {
        private DynamicCrystAssetT<T> _mainAsset;
        public DynamicCrystAssetT<T> DynamicAssetT
        {
            get
            {
                return _mainAsset;
            }
            set
            {
                _mainAsset = value;
            }
        }

        public override DynamicCrystAsset DynamicAsset
        {
            get => DynamicAssetT;
            set => DynamicAssetT = new DynamicCrystAssetT<T>(value.Asset);
        }

        public IWeakAssetT<T> MainAssetT => DynamicAssetT;
    }
}
