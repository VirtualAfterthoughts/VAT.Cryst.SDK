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

        public readonly float GetCurl()
        {
            if (phalanges.Length <= 0)
                return 0f;

            return phalanges[0].curl;
        }

        public static FingerPoseData Create(int phalanxCount)
        {
            return new FingerPoseData
            {
                phalanges = new PhalanxPoseData[phalanxCount],
            };
        }
    }
}
