using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class StaticCrystGameObject : StaticCrystAssetT<GameObject>
    {
        public StaticCrystGameObject(string guid) : base(guid) { }
    }
}
