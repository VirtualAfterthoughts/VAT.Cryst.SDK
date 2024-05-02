using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input.Skeleton
{
    public interface IInputArm : IInputLimb
    {
        IInputHand GetHandOrNull();

        IInputJoint GetElbowOrNull();

        IInputJoint GetUpperArmOrNull();
    }
}
