using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public abstract class VirtualControllerOverride : MonoBehaviour
    {
        public abstract void Solve(VirtualControllerData data);
    }
}
