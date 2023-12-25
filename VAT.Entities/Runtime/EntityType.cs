using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Entities
{
    [Flags]
    public enum EntityType
    {
        PLAYER = 1 << 0,
        NPC = 1 << 1,
    }
}
