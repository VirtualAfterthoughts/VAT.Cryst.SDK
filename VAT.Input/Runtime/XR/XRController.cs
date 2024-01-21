using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Input.Unity;
using VAT.Input.XR;

namespace VAT.Input
{
    public class XRController : IInputController
    {
        private readonly UnityTrigger _trigger;
        private readonly UnityTrigger _grip;

        private readonly UnityButton _primaryButton;
        private readonly UnityButton _secondaryButton;
        
        private readonly UnityTrackpad _thumbstick;
        private readonly UnityTrackpad _trackpad;

        public XRController(Handedness handedness, XRInputActions actions)
        {
            switch (handedness)
            {
                case Handedness.LEFT:
                    var left = actions.ControllerLeft;

                    _trigger = new UnityTrigger(left.TriggerAxis, left.TriggerPressed);
                    _grip = new UnityTrigger(left.GripAxis, left.GripForce, left.GripPressed);

                    _primaryButton = new UnityButton(left.PrimaryButtonPressed, left.PrimaryButtonTouched);
                    _secondaryButton = new UnityButton(left.SecondaryButtonPressed, left.SecondaryButtonTouched);

                    _thumbstick = new UnityTrackpad(left.ThumbstickAxis, left.ThumbstickPressed, left.ThumbstickTouched);
                    _trackpad = new UnityTrackpad(left.TrackpadAxis, left.TrackpadPressed, left.TrackpadTouched);
                    break;
                case Handedness.RIGHT:
                    var right = actions.ControllerRight;

                    _trigger = new UnityTrigger(right.TriggerAxis, right.TriggerPressed);
                    _grip = new UnityTrigger(right.GripAxis, right.GripForce, right.GripPressed);

                    _primaryButton = new UnityButton(right.PrimaryButtonPressed, right.PrimaryButtonTouched);
                    _secondaryButton = new UnityButton(right.SecondaryButtonPressed, right.SecondaryButtonTouched);

                    _thumbstick = new UnityTrackpad(right.ThumbstickAxis, right.ThumbstickPressed, right.ThumbstickTouched);
                    _trackpad = new UnityTrackpad(right.TrackpadAxis, right.TrackpadPressed, right.TrackpadTouched);
                    break;
            }
        }

        public bool TryGetGrip(out IInputTrigger grip)
        {
            grip = _grip;
            return true;
        }

        public bool TryGetPrimaryButton(out IInputButton primaryButton)
        {
            primaryButton = _primaryButton;
            return true;
        }

        public bool TryGetSecondaryButton(out IInputButton secondaryButton)
        {
            secondaryButton = _secondaryButton;
            return true;
        }

        public bool TryGetThumbstick(out IInputTrackpad thumbstick)
        {
            thumbstick = _thumbstick;
            return true;
        }

        public bool TryGetTrackpad(out IInputTrackpad trackpad)
        {
            trackpad = _trackpad;
            return true;
        }

        public bool TryGetTrigger(out IInputTrigger trigger)
        {
            trigger = _trigger;
            return true;
        }
    }
}
