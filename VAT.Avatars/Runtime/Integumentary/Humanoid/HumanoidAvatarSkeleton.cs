using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;
using VAT.Avatars.Art;

namespace VAT.Avatars.Integumentary
{
    public sealed class HumanoidAvatarSkeleton : AvatarSkeletonT<HumanoidDataSkeleton, HumanoidPhysSkeleton, HumanoidArtSkeleton>
    {
        private readonly HumanoidDataSkeleton _dataSkeleton;
        public override HumanoidDataSkeleton GenericDataBoneSkeleton => _dataSkeleton;

        private readonly HumanoidPhysSkeleton _physSkeleton;
        public override HumanoidPhysSkeleton GenericPhysBoneSkeleton => _physSkeleton;

        private readonly HumanoidArtSkeleton _artSkeleton;
        public override HumanoidArtSkeleton GenericArtBoneSkeleton => _artSkeleton;

        public HumanoidAvatarSkeleton(
            HumanoidDataSkeleton dataSkeleton,
            HumanoidPhysSkeleton physSkeleton,
            HumanoidArtSkeleton artSkeleton)
        {

            _dataSkeleton = dataSkeleton;
            _physSkeleton = physSkeleton;
            _artSkeleton = artSkeleton;
        }
    }
}
