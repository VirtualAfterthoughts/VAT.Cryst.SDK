using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;

namespace VAT.Input.Skeleton
{
    public interface IInputHand : IInputJoint
    {
        IInputController GetInputControllerOrNull();
    }
}
