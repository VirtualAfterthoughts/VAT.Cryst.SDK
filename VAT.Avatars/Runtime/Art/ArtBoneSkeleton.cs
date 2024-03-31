using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.REWORK;

using VAT.Shared.Data;

namespace VAT.Avatars.Art
{
    public abstract class ArtBoneSkeleton : ISkeleton {
        public abstract int BoneGroupCount { get; }
        public abstract IBoneGroup[] BoneGroups { get; }

        public abstract void Initiate();
        public virtual void Deinitiate() { }

        public abstract void Solve();

        public abstract IBone GetHead();
        public abstract SimpleTransform GetEyeCenter();
    }
}
