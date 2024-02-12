using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.REWORK;

namespace VAT.Avatars
{
    public interface IThumbGroup : IBoneGroup
    {
        public IBone Proximal { get; }
        public IBone Middle { get; }
        public IBone Distal { get; }
    }
}
