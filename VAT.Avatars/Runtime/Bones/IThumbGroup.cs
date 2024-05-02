using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Bones;

namespace VAT.Avatars.Bones
{
    public interface IThumbGroup : IBoneGroup
    {
        public IBone Proximal { get; }
        public IBone Middle { get; }
        public IBone Distal { get; }
    }
}
