using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Avatars.REWORK
{
    public interface IBoneGroup
    {
        IBone[] Bones { get; }
        int BoneCount { get; }

        IBoneGroup[] SubGroups { get; }
        int SubGroupCount { get; }
    }
}
