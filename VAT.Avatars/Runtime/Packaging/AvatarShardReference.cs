using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Packaging;

using VAT.Packaging;

namespace VAT.Avatars
{
    [Serializable]
    public class AvatarShardReference : ShardReferenceT<AvatarShard>
    {
#if UNITY_EDITOR
        public override Type EditorShardType => typeof(AvatarShard);
#endif
    }
}
