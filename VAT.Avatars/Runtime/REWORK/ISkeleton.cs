using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.REWORK
{
    public interface ISkeleton
    {
        IBoneGroup[] BoneGroups { get; }
        int BoneGroupCount { get; }

        void Initiate();
        void Deinitiate();

        void Solve();

        IBone GetHead();
        SimpleTransform GetEyeCenter();
    }
}
