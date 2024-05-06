using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Combat
{
    public interface IAttackReceiver
    {
        void ReceiveAttack(Attack attack);
    }
}
