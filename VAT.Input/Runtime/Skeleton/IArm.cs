using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input.Skeleton
{
    public interface IArm : ILimb
    {
        IHand GetHandOrNull();

        IJoint GetElbowOrNull();

        IJoint GetUpperArmOrNull();
    }
}
