using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.REWORK;
using VAT.Avatars.Skeletal;

using VAT.Input;

namespace VAT.Avatars.Integumentary {
    public readonly struct AvatarArm {
        public Handedness Handedness { get; }
        public IArmGroup DataArm { get; }
        public IArmGroup PhysArm { get; }

        public AvatarArm(Handedness handedness, IArmGroup dataArm, IArmGroup physArm)
        {
            Handedness = handedness;
            DataArm = dataArm;
            PhysArm = physArm;
        }
    }
}
