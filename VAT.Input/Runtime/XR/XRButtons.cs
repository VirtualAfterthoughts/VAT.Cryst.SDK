using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using VAT.Cryst.Extensions;

namespace VAT.Input.XR
{
    public class XRButton : IInputButton
    {
        private bool _isPressed;
        private bool _isTouched;

        public XRButton(InputAction pressedAction, InputAction touchedAction)
        {
            pressedAction.AddCallback(Pressed_callback);
            touchedAction.AddCallback(Touched_callback);
        }

        private void Pressed_callback(InputAction.CallbackContext context)
        {
            _isPressed = context.ReadValue<bool>();
        }

        private void Touched_callback(InputAction.CallbackContext context)
        {
            _isTouched = context.ReadValue<bool>();
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

    public class XRTrackpad : IInputTrackpad
    {
        private Vector2 _axis;
        private bool _isPressed;
        private bool _isTouched;

        public XRTrackpad(InputAction axisAction, InputAction pressedAction, InputAction touchedAction)
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
            _isPressed = context.ReadValue<bool>();
        }

        private void Touched_callback(InputAction.CallbackContext context)
        {
            _isTouched = context.ReadValue<bool>();
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

    public class XRTrigger : IInputTrigger
    {
        private float _axis;
        private float _force;
        private bool _isPressed;

        public XRTrigger(InputAction axisAction, InputAction pressedAction) 
            : this(axisAction, axisAction, pressedAction)  { }

        public XRTrigger(InputAction axisAction, InputAction forceAction, InputAction pressedAction)
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
            _isPressed = context.ReadValue<bool>();
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
