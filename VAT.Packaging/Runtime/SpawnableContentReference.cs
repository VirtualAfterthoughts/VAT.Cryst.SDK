using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class SpawnableContentReference : ContentReferenceT<ISpawnableContent>
    {
#if UNITY_EDITOR
        public override Type EditorContentType => typeof(StaticSpawnableContent);
#endif
    }
}
