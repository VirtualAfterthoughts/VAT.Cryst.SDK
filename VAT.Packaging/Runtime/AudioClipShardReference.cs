using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class AudioClipShardReference : ShardReferenceT<IAudioClipShard>
    {
#if UNITY_EDITOR
        public override Type EditorShardType => typeof(StaticAudioClipShard);
#endif
    }
}
