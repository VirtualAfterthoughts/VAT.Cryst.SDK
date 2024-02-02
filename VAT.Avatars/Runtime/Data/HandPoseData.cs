using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars
{
    [Serializable]
    public struct HandPoseData
    {
        public ThumbPoseData[] thumbs;

        public FingerPoseData[] fingers;
    }
}
