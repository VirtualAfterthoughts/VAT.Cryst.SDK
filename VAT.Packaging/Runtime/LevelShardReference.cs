using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class LevelShardReference : ShardReferenceT<ILevelShard>
    {
#if UNITY_EDITOR
        public override Type EditorShardType => typeof(StaticLevelShard);
#endif

        public LevelShardReference() 
        {
            _address = Address.EMPTY;
        }

        public LevelShardReference(Address address)
        {
            _address = address;
        }
    }
}
