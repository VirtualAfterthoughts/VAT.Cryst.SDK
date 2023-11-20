using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class ScriptableObjectContentReference : ContentReferenceT<IScriptableObjectContent>
    {
#if UNITY_EDITOR
        public override Type EditorContentType => typeof(StaticScriptableObjectContent);
#endif
    }
}
