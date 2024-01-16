using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Proportions;

namespace VAT.Avatars.Skeletal {
    public abstract class HumanoidBoneGroup : DataBoneGroup {
        public abstract void WriteProportions(HumanoidProportions proportions);
    }
}
