using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Zones
{
    [Flags]
    public enum ZoneState
    {
        NONE = 0,
        PRIMARY = 1 << 0,
        SECONDARY = 1 << 1,
    }
}
