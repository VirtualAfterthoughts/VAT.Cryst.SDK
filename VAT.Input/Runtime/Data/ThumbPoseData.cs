using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars
{
    [Serializable]
    public struct ThumbPoseData
    {
        [Range(-1f, 1f)]
        public float stretched;

        [Range(-1f, 1f)]
        public float spread;

        [Range(-1f, 1f)]
        public float twist;

        public PhalanxPoseData[] phalanges;

        public static ThumbPoseData Create(int phalanxCount)
        {
            return new ThumbPoseData
            {
                phalanges = new PhalanxPoseData[phalanxCount],
            };
        }
    }
}
