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
        public SpawnableShardReference shardReference;
        public SpawnRules rules;

        public Spawnable(Address address)
        {
            shardReference = new SpawnableShardReference(address);
            rules = SpawnRules.Default;
        }

        public Spawnable(SpawnableShardReference shardReference)
        {
            this.shardReference = shardReference;
            rules = SpawnRules.Default;
        }
    }
}
