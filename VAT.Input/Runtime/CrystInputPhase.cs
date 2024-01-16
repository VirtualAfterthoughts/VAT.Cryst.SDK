using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input
{
    public enum CrystInputPhase
    {
        STARTED = 1 << 0,
        PERFORMED = 1 << 1,
        CANCELED = 1 << 2,
    }
}
