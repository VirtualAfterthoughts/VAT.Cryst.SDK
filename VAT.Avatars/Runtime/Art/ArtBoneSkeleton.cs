using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.REWORK;

namespace VAT.Avatars.Art
{
    public abstract class ArtBoneSkeleton : ISkeleton {
        public abstract int BoneGroupCount { get; }
        public abstract IBoneGroup[] BoneGroups { get; }

        public abstract void Initiate();
        public virtual void Deinitiate() { }

        public abstract void Solve();
    }

    public abstract class ArtBoneSkeletonT<TArtDescriptor, TSkeleton> : ArtBoneSkeleton 
        where TArtDescriptor : IArtDescriptor {

        public abstract void WriteTransforms(TArtDescriptor artDescriptor);

        public abstract void WriteData(TSkeleton skeleton);

        public abstract void WriteOffsets(TSkeleton skeleton);
    }
}
