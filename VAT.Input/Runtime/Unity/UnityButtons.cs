using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using VAT.Cryst.Extensions;

namespace VAT.Input.Unity
{
    public class UnityButton : IInputButton
    {
        private bool _isPressed;
        private bool _isTouched;

        public UnityButton(InputAction pressedAction, InputAction touchedAction)
        {
            pressedAction.AddCallback(Pressed_callback);
            touchedAction.AddCallback(Touched_callback);
        }

        private void Pressed_callback(InputAction.CallbackContext context)
        {
            _isPressed = context.performed;
        }

        private void Touched_callback(InputAction.CallbackContext context)
        {
            _isTouched = context.performed;
        }

        public bool GetPressed()
        {
            return _isPressed;
        }

        public bool GetTouched()
        {
            return _isTouched;
        }
    }

    public class UnityTrackpad : IInputTrackpad
    {
        private Vector2 _axis;
        private bool _isPressed;
        private bool _isTouched;

        public UnityTrackpad(InputAction axisAction, InputAction pressedAction, InputAction touchedAction)
        {
            axisAction.AddCallback(Axis_callback);
            pressedAction.AddCallback(Pressed_callback);
            touchedAction.AddCallback(Touched_callback);
        }

        private void Axis_callback(InputAction.CallbackContext context)
        {
            _axis = context.ReadValue<Vector2>();
        }

        private void Pressed_callback(InputAction.CallbackContext context)
        {
            _isPressed = context.performed;
        }

        private void Touched_callback(InputAction.CallbackContext context)
        {
            _isTouched = context.performed;
        }

        public Vector2 GetAxis()
        {
            return _axis;
        }

        public bool GetPressed()
        {
            return _isPressed;
        }

        public bool GetTouched()
        {
            return _isTouched;
        }
    }

    public class UnityTrigger : IInputTrigger
    {
        private float _axis;
        private float _force;
        private bool _isPressed;

        public UnityTrigger(InputAction axisAction, InputAction pressedAction) 
            : this(axisAction, axisAction, pressedAction)  { }

        public UnityTrigger(InputAction axisAction, InputAction forceAction, InputAction pressedAction)
        {
            axisAction.AddCallback(Axis_callback);
            forceAction.AddCallback(Force_callback);
            pressedAction.AddCallback(Pressed_callback);
        }

        private void Axis_callback(InputAction.CallbackContext context)
        {
            _axis = context.ReadValue<float>();
        }

        private void Force_callback(InputAction.CallbackContext context)
        {
            _force = context.ReadValue<float>();
        }

        private void Pressed_callback(InputAction.CallbackContext context)
        {
            _isPressed = context.performed;
        }

        public float GetAxis()
        {
            return _axis;
        }

        public float GetForce()
        {
            return _force;
        }

        public bool GetPressed()
        {
            return _isPressed;
        }

        public bool GetTouched()
        {
            return false;
        }
    }
}
