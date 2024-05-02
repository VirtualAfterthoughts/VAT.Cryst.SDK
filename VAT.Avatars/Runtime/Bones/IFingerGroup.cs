using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Bones
{
    public interface IFingerGroup : IBoneGroup
    {
        public IBone Proximal { get; }
        public IBone Middle { get; }
        public IBone Distal { get; }
    }
}
