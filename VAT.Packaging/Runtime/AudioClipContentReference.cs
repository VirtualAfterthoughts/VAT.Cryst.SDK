using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class AudioClipContentReference : ContentReferenceT<IAudioClipContent>
    {
#if UNITY_EDITOR
        public override Type EditorContentType => typeof(StaticAudioClipContent);
#endif
    }
}
