using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    public abstract class AssetShard : Shard, IAssetShard
    {
        public abstract IWeakAsset MainAsset { get; }
    }
}
