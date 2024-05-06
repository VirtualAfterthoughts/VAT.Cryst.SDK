using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Input.Data
{
    [Serializable]
    public struct PhalanxPoseData
    {
        [Range(-1f, 1f)]
        public float curl;
    }
}
