using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.REWORK;

using VAT.Input;

namespace VAT.Avatars
{
    public interface ILimbGroup : IBoneGroup
    {
        Handedness Handedness { get; }
    }
}
