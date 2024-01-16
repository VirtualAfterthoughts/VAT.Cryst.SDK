using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;

using VAT.Input;

namespace VAT.Avatars.Integumentary {
    public interface IAvatarHand {
        public Handedness Handedness { get; }
        public DataBone DataHand { get; }
        public PhysBone PhysHand { get; }
    }
}
