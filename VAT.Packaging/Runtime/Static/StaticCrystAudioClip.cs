using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class StaticCrystAudioClip : StaticCrystAssetT<AudioClip>
    {
        public StaticCrystAudioClip(string guid) : base(guid) { }
    }
}
