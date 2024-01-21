using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.XR;

namespace VAT.Input
{
    public class XRController : IInputController
    {
        private readonly XRTrigger _trigger;
        private readonly XRTrigger _grip;

        private readonly XRButton _primaryButton;
        private readonly XRButton _secondaryButton;
        
        private readonly XRTrackpad _thumbstick;
        private readonly XRTrackpad _trackpad;

        public XRController(Handedness handedness, XRInputActions actions)
        {
            switch (handedness)
            {
                case Handedness.LEFT:
                    var left = actions.ControllerLeft;

                    _trigger = new XRTrigger(left.TriggerAxis, left.TriggerPressed);
                    _grip = new XRTrigger(left.GripAxis, left.GripForce, left.GripPressed);

                    _primaryButton = new XRButton(left.PrimaryButtonPressed, left.PrimaryButtonTouched);
                    _secondaryButton = new XRButton(left.SecondaryButtonPressed, left.SecondaryButtonTouched);

                    _thumbstick = new XRTrackpad(left.ThumbstickAxis, left.ThumbstickPressed, left.ThumbstickTouched);
                    _trackpad = new XRTrackpad(left.TrackpadAxis, left.TrackpadPressed, left.TrackpadTouched);
                    break;
                case Handedness.RIGHT:
                    var right = actions.ControllerRight;

                    _trigger = new XRTrigger(right.TriggerAxis, right.TriggerPressed);
                    _grip = new XRTrigger(right.GripAxis, right.GripForce, right.GripPressed);

                    _primaryButton = new XRButton(right.PrimaryButtonPressed, right.PrimaryButtonTouched);
                    _secondaryButton = new XRButton(right.SecondaryButtonPressed, right.SecondaryButtonTouched);

                    _thumbstick = new XRTrackpad(right.ThumbstickAxis, right.ThumbstickPressed, right.ThumbstickTouched);
                    _trackpad = new XRTrackpad(right.TrackpadAxis, right.TrackpadPressed, right.TrackpadTouched);
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
