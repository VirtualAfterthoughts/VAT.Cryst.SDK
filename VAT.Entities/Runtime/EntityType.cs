using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Entities
{
    public enum EntityType
    {
        NONE = 1 << 0,
        PLAYER = 1 << 1,
        NPC = 1 << 2,
    }
}
