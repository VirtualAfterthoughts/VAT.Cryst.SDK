using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Art
{
    public abstract class ArtBoneSkeleton : ISkeleton {
        public abstract int BoneGroupCount { get; }

        public abstract void Initiate();

        public abstract void Solve();
    }

    public abstract class ArtBoneSkeletonT<TGroup, TArtDescriptor, TDataSkeleton, TPhysSkeleton> : ArtBoneSkeleton 
        where TGroup : ArtBoneGroup
        where TArtDescriptor : IArtDescriptor {

        public abstract TGroup[] BoneGroups { get; }

        public abstract void WriteTransforms(TArtDescriptor artDescriptor);

        public abstract void WriteData(TDataSkeleton dataSkeleton, TPhysSkeleton physSkeleton);

        public abstract void WriteOffsets(TDataSkeleton dataSkeleton);
    }
}
