using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Packaging
{
    public enum AssetLoadStatus
    {
        IDLE = 1 << 0,
        LOADING = 1 << 1,
        DONE = 1 << 2,
        FAILED = 1 << 3,
    }
}
