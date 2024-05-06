using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Characters
{
    using System;

    using VAT.Avatars.Integumentary;

    using VAT.Interaction;

    public interface IAvatarRig : ICrystRig
    {
        event Action<Avatar> OnSwitchedAvatar, OnExitingAvatar;

        IInteractor[] GetCurrentInteractors();

        Avatar CurrentAvatar { get; }

        void SwitchAvatar(Avatar avatar);
    }
}
