using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars
{
    public interface IArm : ILimb
    {
        IHand GetHandOrNull();

        IJoint GetElbowOrNull();

        IJoint GetUpperArmOrNull();
    }
}
