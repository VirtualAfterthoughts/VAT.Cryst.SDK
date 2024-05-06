using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Entities
{
    [Flags]
    public enum CrystEntityType
    {
        /// <summary>
        /// A generalized entity, such as an object or particle.
        /// </summary>
        MISC = 1 << 0,
        
        /// <summary>
        /// A player.
        /// </summary>
        PLAYER = 1 << 1,

        /// <summary>
        /// A non-player character.
        /// </summary>
        NPC = 1 << 2,
    }
}
