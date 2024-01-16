using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input
{
    public class InputThumbstick
    {
        private readonly CrystInputActionT<Vector2> _axisAction;
        private readonly CrystInputActionT<bool> _pressedAction;

        public InputThumbstick(CrystInputActionT<Vector2> axisAction, CrystInputActionT<bool> pressedAction)
        {
            _axisAction = axisAction;
            _pressedAction = pressedAction;
        }

        public CrystInputActionT<bool> GetPressedAction()
        {
            return _pressedAction;
        }

        public CrystInputActionT<Vector2> GetAxisAction()
        {
            return _axisAction;
        }
    }
}
