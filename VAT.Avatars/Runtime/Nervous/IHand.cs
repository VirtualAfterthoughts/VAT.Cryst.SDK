using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;

namespace VAT.Avatars
{
    public interface IHand : IJoint
    {
        bool TryGetInputController(out IInputController controller);
    }
}