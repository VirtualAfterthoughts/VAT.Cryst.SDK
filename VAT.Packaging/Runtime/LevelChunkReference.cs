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
        private LevelContentReference _contentReference;

        [SerializeField]
        private string _chunkName = string.Empty;

        public bool TryGetChunk(out StaticCrystChunk chunk)
        {
            if (_contentReference.TryGetContent(out var content) && content is StaticLevelContent levelContent)
            {
                return levelContent.TryGetChunk(_chunkName, out chunk);
            }

            chunk = null;
            return false;
        }
    }
}
