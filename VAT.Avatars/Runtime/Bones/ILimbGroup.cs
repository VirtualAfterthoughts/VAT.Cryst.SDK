using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Cryst.Delegates;
using VAT.Input;
using VAT.Shared.Data;

namespace VAT.Avatars.Bones
{
    public interface ILimbGroup : IBoneGroup
    {
        Handedness Handedness { get; }

        SimpleTransform EndTarget { get; }

        event TargetProcessorCallback OnProcessTarget;
    }
}
