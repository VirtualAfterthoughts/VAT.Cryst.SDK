using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;
using VAT.Avatars.Vitals;

namespace VAT.Avatars.Integumentary
{
    public sealed class HumanoidAvatarAnatomy : AvatarAnatomyT<HumanoidAvatarSkeleton, HumanoidVitals> {
        private readonly HumanoidAvatarSkeleton _skeleton;
        public override HumanoidAvatarSkeleton GenericSkeleton => _skeleton;

        private readonly HumanoidVitals _vitals;
        public override HumanoidVitals GenericVitals => _vitals;

        public HumanoidAvatarAnatomy(
            HumanoidAvatarSkeleton skeleton,
            HumanoidVitals vitals) {

            _skeleton = skeleton;
            _vitals = vitals;
        }
    }
}
