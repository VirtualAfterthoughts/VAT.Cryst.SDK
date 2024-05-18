using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Bones
{
    public interface IFingerGroup : IBoneGroup
    {
        IBone Proximal { get; }
        IBone Middle { get; }
        IBone Distal { get; }
    }
}
