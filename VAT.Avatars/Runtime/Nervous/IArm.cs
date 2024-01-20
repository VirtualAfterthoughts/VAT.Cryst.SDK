using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars
{
    public interface IArm : ILimb
    {
        bool TryGetHand(out IHand hand);

        bool TryGetElbow(out IJoint elbow);

        bool TryGetUpperArm(out IJoint upperArm);
    }
}
