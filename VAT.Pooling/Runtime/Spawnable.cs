using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

namespace VAT.Pooling
{
    [Serializable]
    public struct Spawnable
    {
        public SpawnableContentReference contentReference;
        public SpawnRules rules;
    }
}
