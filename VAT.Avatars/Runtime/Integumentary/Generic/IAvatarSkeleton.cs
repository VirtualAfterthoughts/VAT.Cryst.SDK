using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;
using VAT.Avatars.Art;

namespace VAT.Avatars.Integumentary
{
    public interface IAvatarSkeleton {
        public DataBoneSkeleton DataBoneSkeleton { get; }
        public PhysBoneSkeleton PhysBoneSkeleton { get; }
        public ArtBoneSkeleton ArtBoneSkeleton { get; }
    }

    public abstract class AvatarSkeletonT<TDataSkeleton, TPhysSkeleton, TArtSkeleton> : IAvatarSkeleton
    where TDataSkeleton : DataBoneSkeleton
    where TPhysSkeleton : PhysBoneSkeleton
    where TArtSkeleton : ArtBoneSkeleton
    {

        public DataBoneSkeleton DataBoneSkeleton => GenericDataBoneSkeleton;
        public PhysBoneSkeleton PhysBoneSkeleton => GenericPhysBoneSkeleton;
        public ArtBoneSkeleton ArtBoneSkeleton => GenericArtBoneSkeleton;

        public abstract TDataSkeleton GenericDataBoneSkeleton { get; }
        public abstract TPhysSkeleton GenericPhysBoneSkeleton { get; }
        public abstract TArtSkeleton GenericArtBoneSkeleton { get; }
    }
}
