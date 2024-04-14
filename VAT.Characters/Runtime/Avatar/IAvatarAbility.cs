using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Characters
{
    using VAT.Avatars.Integumentary;

    public interface IAvatarAbility
    {
        void OnInitiateAvatar(Avatar avatar, IAvatarRig rig);

        void OnDeinitiateAvatar(Avatar avatar, IAvatarRig rig);
    }
}
