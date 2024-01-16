using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Input
{
    public abstract class InputHand
    {
        public abstract Handedness Handedness { get; }

        public abstract InputFinger[] Fingers { get; }

        public abstract bool TryGetThumbstick(out InputThumbstick thumbstick);
        public abstract bool TryGetTrackpad(out InputThumbstick trackpad);

        public abstract bool TryGetTrigger(out InputTrigger trigger);
        public abstract bool TryGetGrip(out InputTrigger grip);

        public abstract bool TryGetPrimaryAction(out InputButton button);
        public abstract bool TryGetSecondaryAction(out InputButton button);

    }
}
