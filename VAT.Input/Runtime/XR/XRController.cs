using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Data;

using VAT.Input.Haptic;
using VAT.Input.Unity;
using VAT.Input.XR;

using VAT.Shared.Data;

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

        private readonly XRHaptor _haptor;

        private readonly XRHand _hand;

        private readonly HandActions _handActions;

        public XRController(Handedness handedness, XRInputActions actions, XRHand hand)
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

                    _haptor = new XRHaptor(left.Haptic, handedness);
                    break;
                case Handedness.RIGHT:
                    var right = actions.ControllerRight;

                    _trigger = new UnityTrigger(right.TriggerAxis, right.TriggerPressed);
                    _grip = new UnityTrigger(right.GripAxis, right.GripForce, right.GripPressed);

                    _primaryButton = new UnityButton(right.PrimaryButtonPressed, right.PrimaryButtonTouched);
                    _secondaryButton = new UnityButton(right.SecondaryButtonPressed, right.SecondaryButtonTouched);

                    _thumbstick = new UnityTrackpad(right.ThumbstickAxis, right.ThumbstickPressed, right.ThumbstickTouched);
                    _trackpad = new UnityTrackpad(right.TrackpadAxis, right.TrackpadPressed, right.TrackpadTouched);

                    _haptor = new XRHaptor(right.Haptic, handedness);
                    break;
            }

            _hand = hand;

            _handActions = new HandActions();
        }

        public IInputHaptor GetHaptor()
        {
            return _haptor;
        }

        public IInputTrigger GetGrip()
        {
            return _grip;
        }

        public IInputButton GetPrimaryButton()
        {
            return _primaryButton;
        }

        public IInputButton GetSecondaryButton()
        {
            return _secondaryButton;
        }

        public IInputTrackpad GetThumbstick()
        {
            return _thumbstick;
        }

        public IInputTrackpad GetTrackpad()
        {
            return _trackpad;
        }

        public IInputTrigger GetTrigger()
        {
            return _trigger;
        }

        public bool HasForceSensor()
        {
            return true;
        }

        public void Update()
        {
            UpdateActions(_handActions, this);
        }

        private void UpdateActions(HandActions actions, XRController inputController)
        {
            var blendPose = inputController.GetHandPose();

            float secondaryCurl = 0f;
            for (var i = 1; i < blendPose.fingers.Length; i++)
            {
                secondaryCurl = Mathf.Max(secondaryCurl, blendPose.fingers[i].GetCurl());
            }

            var trigger = inputController.GetTrigger();
            bool triggerPull = trigger?.GetAxis() > 0.3f;
            bool gripPull = secondaryCurl > 0.7f;

            bool triggerClick = (trigger?.GetPressed()).GetValueOrDefault();

            bool gripPose = gripPull || triggerPull;
            bool interactPose = gripPull && triggerClick;

            actions.GrabAction.State = gripPose;
            actions.AbilityGrabAction.State = interactPose;

            var primaryButton = inputController.GetPrimaryButton()?.GetPressed();
            var secondaryButton = inputController.GetSecondaryButton()?.GetPressed();

            actions.PrimaryAction.State = primaryButton.GetValueOrDefault();
            actions.SecondaryAction.State = secondaryButton.GetValueOrDefault();
        }

        public HandPoseData GetHandPose()
        {
            return _hand.GetHandPose();
        }

        public HandActions GetActions()
        {
            return _handActions;
        }
    }
}
