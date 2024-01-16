using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Proportions {
    public struct HumanoidGeneralProportions {
        public float height;
    }

    [Serializable]
    public struct HumanoidProportions : ISkeletonProportions {
        [NonSerialized]
        public HumanoidGeneralProportions generalProportions;

        public HumanoidNeckProportions neckProportions;

        public HumanoidSpineProportions spineProportions;

        public HumanoidArmProportions leftArmProportions;
        public HumanoidArmProportions rightArmProportions;

        public HumanoidLegProportions leftLegProportions;
        public HumanoidLegProportions rightLegProportions;
    }
}
