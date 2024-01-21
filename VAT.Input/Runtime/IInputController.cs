using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Input
{
    public interface IInputController
    {
        bool TryGetTrigger(out IInputTrigger trigger);

        bool TryGetGrip(out IInputTrigger grip);

        bool TryGetThumbstick(out IInputTrackpad thumbstick);

        bool TryGetTrackpad(out IInputTrackpad trackpad);

        bool TryGetPrimaryButton(out IInputButton primaryButton);

        bool TryGetSecondaryButton(out IInputButton secondaryButton);
    }
}
