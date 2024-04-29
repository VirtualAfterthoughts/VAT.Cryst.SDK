using Newtonsoft.Json;
using System;

using UnityEngine;

namespace VAT.Packaging
{
    public interface IShardReference
    {
        Address Address { get; }
        bool TryGetShard(out IShard shard);
    }

    [Serializable]
    public class ShardReference : IShardReference
    {
        [SerializeField]
        protected Address _address = Address.EMPTY;

        [JsonProperty("address")]
        public Address Address { get { return _address; } set { _address = value; } }

#if UNITY_EDITOR
        public virtual Type EditorShardType => typeof(Shard);
#endif

        public bool TryGetShard(out IShard shard)
        {
            shard = null;

            if (!AssetPackager.IsReady)
                return false;

            return AssetPackager.Instance.TryGetShard(Address, out shard);
        }
    }

    [Serializable]
    public class ShardReferenceT<T> : ShardReference where T : IShard
    {
#if UNITY_EDITOR
        public override Type EditorShardType => typeof(T);
#endif

        public bool TryGetShard(out T shard)
        {
            shard = default;
            base.TryGetShard(out var otherShard);

            if (otherShard is T result)
            {
                shard = result;
                return true;
            }

            return false;
        }
    }
}
