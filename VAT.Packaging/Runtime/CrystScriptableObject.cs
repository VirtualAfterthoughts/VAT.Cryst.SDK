using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class StaticCrystScriptableObject : StaticCrystAssetT<ScriptableObject>
    {
        public StaticCrystScriptableObject(string guid) : base(guid) { }
    }
}
