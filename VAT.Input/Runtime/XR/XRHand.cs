using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.XR;

namespace VAT.Input
{
    public class XRHand : InputHand
    {
        private readonly Handedness _handedness;
        public override Handedness Handedness => _handedness;

        public override InputFinger[] Fingers => throw new System.NotImplementedException();

        private readonly InputTrigger _trigger;
        private readonly InputTrigger _grip;

        private readonly InputButton _primaryAction;
        private readonly InputButton _secondaryAction;

        private readonly InputThumbstick _thumbstick;
        private readonly InputThumbstick _trackpad;

        private readonly bool _valid;

        public XRHand(XRInputActions actions, Handedness handedness)
        {
            _handedness = handedness;

            switch (handedness)
            {
                default:
                    _valid = false;
                    break;
                case Handedness.LEFT:
                    var left = actions.XRControllerLeft;

                    _trigger = new InputTrigger(left.Trigger.CreateCrystInputAction<float>(), left.TriggerPressed.CreateCrystInputAction<bool>());
                    _grip = new InputTrigger(left.Grip.CreateCrystInputAction<float>(), left.GripPressed.CreateCrystInputAction<bool>());

                    _primaryAction = new InputButton(left.PrimaryAction.CreateCrystInputAction<bool>());
                    _secondaryAction = new InputButton(left.SecondaryAction.CreateCrystInputAction<bool>());

                    _thumbstick = new InputThumbstick(left.ThumbstickAxis.CreateCrystInputAction<Vector2>(), left.ThumbstickPressed.CreateCrystInputAction<bool>());
                    _trackpad = new InputThumbstick(left.TrackpadAxis.CreateCrystInputAction<Vector2>(), left.TrackpadPressed.CreateCrystInputAction<bool>());

                    _valid = true;
                    break;
                case Handedness.RIGHT:
                    var right = actions.XRControllerRight;

                    _trigger = new InputTrigger(right.Trigger.CreateCrystInputAction<float>(), right.TriggerPressed.CreateCrystInputAction<bool>());
                    _grip = new InputTrigger(right.Grip.CreateCrystInputAction<float>(), right.GripPressed.CreateCrystInputAction<bool>());

                    _primaryAction = new InputButton(right.PrimaryAction.CreateCrystInputAction<bool>());
                    _secondaryAction = new InputButton(right.SecondaryAction.CreateCrystInputAction<bool>());

                    _thumbstick = new InputThumbstick(right.ThumbstickAxis.CreateCrystInputAction<Vector2>(), right.ThumbstickPressed.CreateCrystInputAction<bool>());
                    _trackpad = new InputThumbstick(right.TrackpadAxis.CreateCrystInputAction<Vector2>(), right.TrackpadPressed.CreateCrystInputAction<bool>());

                    _valid = true;
                    break;
            }
        }

        public override bool TryGetGrip(out InputTrigger grip)
        {
            grip = _grip;
            return _valid;
        }

        public override bool TryGetPrimaryAction(out InputButton button)
        {
            button = _primaryAction;
            return _valid;
        }

        public override bool TryGetSecondaryAction(out InputButton button)
        {
            button = _secondaryAction;
            return _valid;
        }

        public override bool TryGetThumbstick(out InputThumbstick thumbstick)
        {
            thumbstick = _thumbstick;
            return _valid;
        }

        public override bool TryGetTrackpad(out InputThumbstick trackpad)
        {
            trackpad = _trackpad;
            return _valid;
        }

        public override bool TryGetTrigger(out InputTrigger trigger)
        {
            trigger = _trigger;
            return _valid;
        }
    }
}
