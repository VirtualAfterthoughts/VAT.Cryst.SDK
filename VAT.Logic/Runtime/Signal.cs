using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    [Serializable]
    public struct Signal
    {
        public static readonly Signal Identity = new()
        {
            value = 0f,
        };

        public float value;
    }
}
