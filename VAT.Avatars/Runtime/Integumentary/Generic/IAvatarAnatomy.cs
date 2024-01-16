using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;
using VAT.Avatars.Vitals;

namespace VAT.Avatars.Integumentary {
    public interface IAvatarAnatomy {
        public IAvatarSkeleton Skeleton { get; }
        public ISkeletonVitals Vitals { get; }
    }

    public abstract class AvatarAnatomyT<TSkeleton, TVitals> : IAvatarAnatomy
        where TSkeleton : IAvatarSkeleton
        where TVitals : ISkeletonVitals {

        public IAvatarSkeleton Skeleton => GenericSkeleton;
        public ISkeletonVitals Vitals => GenericVitals;

        public abstract TSkeleton GenericSkeleton { get; }
        public abstract TVitals GenericVitals { get; }
    }
}
