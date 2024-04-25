using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Extensions;

namespace VAT.Packaging
{
    [StaticShardIdentifier("Spawnable", typeof(GameObject))]
    public class StaticSpawnableShard : StaticGameObjectShard, ISpawnableShard
    {
#if UNITY_EDITOR
        public override string EditorAssetGroup => "Spawnable";
#endif
    }
}
