using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars.Muscular;
using VAT.Avatars.Proportions;

namespace VAT.Avatars.Vitals {
    public class HumanoidVitalsPayload : IVitalsPayload {
        private HumanoidProportions _proportions;
        private HumanoidPhysSkeleton _skeleton;

        public HumanoidProportions Proportions { get { return _proportions; } }
        public HumanoidPhysSkeleton Skeleton { get { return _skeleton; } }

        public void InjectDependencies(HumanoidProportions proportions, HumanoidPhysSkeleton skeleton) {
            _proportions = proportions;
            _skeleton = skeleton;
        }
    }
}
