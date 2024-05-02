using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

namespace VAT.Avatars.Packaging
{
    [StaticShardIdentifier("Avatar", typeof(GameObject))]
    public class AvatarShard : StaticSpawnableShard
    {
#if UNITY_EDITOR
        public override string EditorAssetGroup => "Avatar";
#endif
    }
}
