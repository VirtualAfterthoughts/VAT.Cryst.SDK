using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Audio
{
    [Serializable]
    public struct AudioPlaySettings
    {
        public static readonly AudioPlaySettings Default = new()
        {
            volume = 1f,
            pitch = 1f,
        };

        public float volume;

        public float pitch;
    }

}
