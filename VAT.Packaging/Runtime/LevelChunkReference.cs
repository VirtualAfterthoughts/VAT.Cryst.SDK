using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class LevelChunkReference
    {
        [SerializeField]
        private LevelShardReference _shardReference;

        [SerializeField]
        private string _chunkName = string.Empty;

        public bool TryGetChunk(out StaticCrystChunk chunk)
        {
            if (_shardReference.TryGetShard(out var content) && content is StaticLevelShard levelContent)
            {
                return levelContent.TryGetChunk(_chunkName, out chunk);
            }

            chunk = null;
            return false;
        }
    }
}
