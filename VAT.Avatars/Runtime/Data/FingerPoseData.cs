using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars
{
    [Serializable]
    public struct FingerPoseData
    {
        [Range(-1f, 1f)]
        public float splay;

        public PhalanxPoseData[] phalanges;
    }
}
