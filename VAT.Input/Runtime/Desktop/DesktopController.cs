using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Data;
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

        private readonly HandActions _handActions;

        private HandPoseData _handPose;

        public DesktopController(Handedness handedness, DesktopInputActions actions)
        {
            switch (handedness)
            {
                case Handedness.LEFT:
                    var left = actions.HandLeft;
                    //
                    //_trigger = new UnityTrigger(left.GripAxis, left.GripAxis);
                    _grip = new UnityTrigger(left.GripAxis, left.GripAxis);
                    //
                    //_primaryButton = new UnityButton(left.PrimaryButtonPressed, left.PrimaryButtonTouched);
                    _secondaryButton = new UnityButton(left.SecondaryButton, left.SecondaryButton);
                    //
                    //_thumbstick = new UnityTrackpad(left.ThumbstickAxis, left.ThumbstickPressed, left.ThumbstickTouched);
                    //_trackpad = new UnityTrackpad(left.TrackpadAxis, left.TrackpadPressed, left.TrackpadTouched);
                    break;
                case Handedness.RIGHT:
                    var right = actions.HandRight;
                    //
                    _trigger = new UnityTrigger(right.TriggerAxis, right.TriggerAxis);
                    _grip = new UnityTrigger(right.GripAxis, right.GripAxis);
                    //
                    //_primaryButton = new UnityButton(right.PrimaryButtonPressed, right.PrimaryButtonTouched);
                    _secondaryButton = new UnityButton(right.SecondaryButton, right.SecondaryButton);
                    //
                    _thumbstick = new UnityTrackpad(right.ThumbstickAxis, right.ThumbstickAxis, right.ThumbstickAxis);
                    //_trackpad = new UnityTrackpad(right.TrackpadAxis, right.TrackpadPressed, right.TrackpadTouched);
                    break;
            }

            _handPose = new HandPoseData()
            {
                fingers = HandPoseCreator.CreateFingers(),
                thumbs = HandPoseCreator.CreateThumbs(),
            };

            _handActions = new HandActions();
        }

        public void Update()
        {
            var trigger = GetTrigger();

            if (trigger != null)
            {
                HandPoseCreator.SetCurls(_handPose.fingers[0].phalanges, trigger.GetAxis());
            }
            var grip = GetGrip();

            float curl = grip.GetAxis();

            for (var i = 1; i < _handPose.fingers.Length; i++)
            {
                HandPoseCreator.SetCurls(_handPose.fingers[i].phalanges, curl);
            }

            for (var i = 0; i < _handPose.thumbs.Length; i++)
            {
                HandPoseCreator.SetCurls(_handPose.thumbs[i].phalanges, curl);
            }

            UpdateActions(_handActions, this);
        }

        private void UpdateActions(HandActions actions, IInputController inputController)
        {
            var blendPose = inputController.GetHandPose();

            float maxCurl = 0f;

            foreach (var finger in blendPose.fingers)
            {
                maxCurl = Mathf.Max(maxCurl, finger.GetCurl());
            }

            float secondaryCurl = 0f;
            for (var i = 1; i < blendPose.fingers.Length; i++)
            {
                secondaryCurl = Mathf.Max(secondaryCurl, blendPose.fingers[i].GetCurl());
            }

            bool gripPose = maxCurl > 0.7f;
            bool interactPose = secondaryCurl > 0.7f && inputController.GetTrigger()?.GetAxis() > 0.7f;

            actions.GrabAction.State = gripPose;
            actions.AbilityGrabAction.State = interactPose;

            var primaryButton = inputController.GetPrimaryButton()?.GetPressed();
            var secondaryButton = inputController.GetSecondaryButton()?.GetPressed();

            actions.PrimaryAction.State = primaryButton.GetValueOrDefault();
            actions.SecondaryAction.State = secondaryButton.GetValueOrDefault();
        }

        public IInputHaptor GetHaptor()
        {
            return null;
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
            return false;
        }

        public HandPoseData GetHandPose()
        {
            return _handPose;
        }

        public HandActions GetActions()
        {
            return _handActions;
        }
    }
}
