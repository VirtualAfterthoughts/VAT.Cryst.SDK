using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Pooling
{
    public enum SpawnMode
    {
        GROW = 0,
        REUSE_OLDEST = 1 << 1,
        REUSE_NEWEST = 1 << 2,
        REUSE_NONE = 1 << 3,
    }

    [Serializable]
    public struct SpawnRules
    {
        public static readonly SpawnRules Default = new(0, SpawnMode.GROW);

        public int maxSpawned;
        public SpawnMode spawnMode;

        public SpawnRules(int maxSpawned, SpawnMode spawnMode)
        {
            this.maxSpawned = maxSpawned;
            this.spawnMode = spawnMode;
        }
    }
}
