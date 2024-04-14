using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Characters
{
    using VAT.Avatars.Integumentary;

    using VAT.Interaction;

    public interface IAvatarRig : ICrystRig
    {
        IInteractor[] GetCurrentInteractors();

        Avatar CurrentAvatar { get; }

        void SwitchAvatar(Avatar avatar);
    }
}
