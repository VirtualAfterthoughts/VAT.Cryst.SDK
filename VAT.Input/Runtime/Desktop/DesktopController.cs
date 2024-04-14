using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Input.Haptic;
using VAT.Input.Unity;

namespace VAT.Input.Desktop
{
    public class DesktopController : IInputController
    {
        private readonly UnityTrigger _trigger;
        private readonly UnityTrigger _grip;

        private readonly UnityButton _primaryButton;
        private readonly UnityButton _secondaryButton;
        
        private readonly UnityTrackpad _thumbstick;
        private readonly UnityTrackpad _trackpad;

        public DesktopController(Handedness handedness, DesktopInputActions actions)
        {
            switch (handedness)
            {
                case Handedness.LEFT:
                    var left = actions.HandLeft;
                    //
                    _trigger = new UnityTrigger(left.GripAxis, left.GripAxis);
                    _grip = new UnityTrigger(left.GripAxis, left.GripAxis);
                    //
                    //_primaryButton = new UnityButton(left.PrimaryButtonPressed, left.PrimaryButtonTouched);
                    //_secondaryButton = new UnityButton(left.SecondaryButtonPressed, left.SecondaryButtonTouched);
                    //
                    //_thumbstick = new UnityTrackpad(left.ThumbstickAxis, left.ThumbstickPressed, left.ThumbstickTouched);
                    //_trackpad = new UnityTrackpad(left.TrackpadAxis, left.TrackpadPressed, left.TrackpadTouched);
                    break;
                case Handedness.RIGHT:
                    var right = actions.HandRight;
                    //
                    _trigger = new UnityTrigger(right.GripAxis, right.GripAxis);
                    _grip = new UnityTrigger(right.GripAxis, right.GripAxis);
                    //
                    //_primaryButton = new UnityButton(right.PrimaryButtonPressed, right.PrimaryButtonTouched);
                    //_secondaryButton = new UnityButton(right.SecondaryButtonPressed, right.SecondaryButtonTouched);
                    //
                    _thumbstick = new UnityTrackpad(right.ThumbstickAxis, right.ThumbstickAxis, right.ThumbstickAxis);
                    //_trackpad = new UnityTrackpad(right.TrackpadAxis, right.TrackpadPressed, right.TrackpadTouched);
                    break;
            }
        }

        public IInputHaptor GetHaptorOrNull()
        {
            return null;
        }

        public IInputTrigger GetGripOrNull()
        {
            return _grip;
        }

        public IInputButton GetPrimaryButtonOrNull()
        {
            return _primaryButton;
        }

        public IInputButton GetSecondaryButtonOrNull()
        {
            return _secondaryButton;
        }

        public IInputTrackpad GetThumbstickOrNull()
        {
            return _thumbstick;
        }

        public IInputTrackpad GetTrackpadOrNull()
        {
            return _trackpad;
        }

        public IInputTrigger GetTriggerOrNull()
        {
            return _trigger;
        }

        public bool HasForceSensor()
        {
            return false;
        }
    }
}
