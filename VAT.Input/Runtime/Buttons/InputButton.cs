using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input
{
    public class InputButton
    {
        private readonly CrystInputActionT<bool> _pressedAction;

        public InputButton(CrystInputActionT<bool> pressedAction)
        {
            _pressedAction = pressedAction;
        }

        public CrystInputActionT<bool> GetPressedAction()
        {
            return _pressedAction;
        }
    }
}
