using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Bones
{
    public interface ISkeleton
    {
        IBoneGroup[] BoneGroups { get; }
        int BoneGroupCount { get; }

        void Initiate();
        void Deinitiate();

        void Solve(float deltaTime);

        IBone GetHead();
        SimpleTransform GetEyeCenter();

        IBone GetAnchor();

        IArmGroup[] GetArms();

        ILegGroup[] GetLegs();
    }
}
