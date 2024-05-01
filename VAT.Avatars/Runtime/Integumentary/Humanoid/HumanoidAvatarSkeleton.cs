using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;
using VAT.Avatars.Art;

namespace VAT.Avatars.Integumentary
{
    public sealed class HumanoidAvatarSkeleton : IAvatarSkeleton
    {
        private readonly HumanoidDataSkeleton _dataSkeleton;
        public HumanoidDataSkeleton DataSkeleton => _dataSkeleton;

        private readonly HumanoidPhysSkeleton _physSkeleton;
        public HumanoidPhysSkeleton PhysSkeleton => _physSkeleton;

        private readonly HumanoidArtSkeleton _artSkeleton;
        public HumanoidArtSkeleton ArtSkeleton => _artSkeleton;

        public HumanoidAvatarSkeleton(HumanoidDataSkeleton data, HumanoidPhysSkeleton physics, HumanoidArtSkeleton art)
        {
            _dataSkeleton = data;
            _physSkeleton = physics;
            _artSkeleton = art;
        }

        public DataBoneSkeleton GetData()
        {
            return _dataSkeleton;
        }

        public PhysBoneSkeleton GetPhysics()
        {
            return _physSkeleton;
        }

        public ArtBoneSkeleton GetArt()
        {
            return _artSkeleton;
        }
    }
}
