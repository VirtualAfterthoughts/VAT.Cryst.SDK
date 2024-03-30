using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Input.Skeleton
{
    public interface ILimb
    {
        IJoint[] Joints { get; }

        int JointCount { get; }
    }
}
