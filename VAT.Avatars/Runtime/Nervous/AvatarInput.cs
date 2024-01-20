using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars
{
    public interface IAvatarInput
    {
        Vector3 GetMovement();

        bool GetJump();
    }
}
