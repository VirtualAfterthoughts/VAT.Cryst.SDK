using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Input
{
    public class InputTrigger
    {
        private readonly CrystInputActionT<float> _axisAction;
        private readonly CrystInputActionT<bool> _pressedAction;

        public InputTrigger(CrystInputActionT<float> axisAction, CrystInputActionT<bool> pressedAction)
        {
            _axisAction = axisAction;
            _pressedAction = pressedAction;
        }

        public CrystInputActionT<bool> GetPressedAction()
        {
            return _pressedAction;
        }

        public CrystInputActionT<float> GetAxisAction()
        {
            return _axisAction;
        }
    }
}
