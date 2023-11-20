using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class LevelContentReference : ContentReferenceT<ILevelContent>
    {
#if UNITY_EDITOR
        public override Type EditorContentType => typeof(StaticLevelContent);
#endif
    }
}
