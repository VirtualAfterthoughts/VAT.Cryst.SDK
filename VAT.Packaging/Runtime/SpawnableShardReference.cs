using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class SpawnableShardReference : ShardReferenceT<ISpawnableShard>
    {
#if UNITY_EDITOR
        public override Type EditorShardType => typeof(StaticSpawnableShard);
#endif

        public SpawnableShardReference(Address address)
        {
            Address = address;
        }
    }
}
