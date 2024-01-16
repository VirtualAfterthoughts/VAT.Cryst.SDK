using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input.Desktop
{
    public class DesktopInput : IBodyInput
    {
        private readonly DesktopInputActions _inputActions;
        private readonly CrystInputActionT<Vector2> _movementAction;

        public DesktopInput()
        {
            _inputActions = new DesktopInputActions();
            _inputActions.Enable();

            _movementAction = _inputActions.Desktop.Movement.CreateCrystInputAction<Vector2>();
        }

        public float GetCrouch()
        {
            return 0f;
        }

        public InputHand[] GetHands()
        {
            return Array.Empty<InputHand>();
        }

        public InputHand[] GetHands(Handedness handedness)
        {
            return Array.Empty<InputHand>();
        }

        public bool GetJump()
        {
            return false;
        }

        public Vector2 GetMovement()
        {
            return _movementAction.Value;
        }

        public float GetTurn()
        {
            return 0f;
        }
    }
}
