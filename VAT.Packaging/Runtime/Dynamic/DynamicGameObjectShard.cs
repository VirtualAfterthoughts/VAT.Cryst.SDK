using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    public class DynamicGameObjectShard : DynamicShardT<GameObject>, IGameObjectShard
    {
        public IWeakAssetT<Mesh> PreviewMesh => null;

        public Bounds Bounds => new();

        public IWeakAssetT<Texture2D> PreviewIcon => null;
    }
}
