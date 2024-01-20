using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Integumentary;
using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;

using VAT.Input;

namespace VAT.Avatars
{
    public class HumanoidAvatarHand : IAvatarHand {
        public Handedness Handedness { get; protected set; }

        public DataBone DataHand { get; protected set; }

        public PhysBone PhysHand { get; protected set; }

        public HumanoidAvatarHand(HumanoidHand hand, HumanoidPhysArm physArm) {
            Handedness = physArm.isLeft ? Handedness.LEFT : Handedness.RIGHT;
            DataHand = hand.Hand;
            PhysHand = physArm.Hand.Hand;
        }
    }
}
