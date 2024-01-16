using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input
{
    /// <summary>
    /// A basic discriminator for left and right.
    /// </summary>
    public enum Handedness {
        NONE = 1 << 0,
        LEFT = 1 << 1,
        RIGHT = 1 << 2,
        BOTH = LEFT | RIGHT,
    }
}
