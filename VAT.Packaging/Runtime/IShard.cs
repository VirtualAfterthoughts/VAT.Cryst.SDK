using System;

using UnityEngine;

using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    [Serializable]
    public struct ShardInfo : IShippableInfo
    {
        [SerializeField]
        [Tooltip("The title of the shard.")]
        private string _title;
        public string Title { get { return _title; } set { _title = value; } }

        [SerializeField]
        [Tooltip("The description of the shard.")]
        private string _description;
        public string Description { get { return _description; } set { _description = value; } }

        [SerializeField]
        [Tooltip("Is this shard unlockable?")]
        private bool _unlockable;
        public bool Unlockable { get { return _unlockable; } set { _unlockable = value; } }

        [SerializeField]
        [Tooltip("Is this shard hidden?")]
        private bool _hidden;
        public bool Hidden { get { return _hidden; } set { _hidden = value; } }
    }

    public interface IShard : IShippable
    {
        ShardInfo ShardInfo { get; set; }

        ICrystal MainCrystal { get; set; }
    }

    public interface IAssetShard : IShard
    {
        IWeakAsset MainAsset { get; }
    }

    public interface IAssetShardT<T> : IAssetShard where T : Object
    {
        IWeakAssetT<T> MainAssetT { get; }
    }
}
