using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars;

using VAT.Input.Haptic;
using VAT.Input.Unity;
using VAT.Shared.Data;

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
                    _trigger = new UnityTrigger(left.GripAxis, left.GripAxis);
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
                    _trigger = new UnityTrigger(right.GripAxis, right.GripAxis);
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
            var trigger = GetTriggerOrNull();

            if (trigger != null)
            {
                HandPoseCreator.SetCurls(_handPose.fingers[0].phalanges, trigger.GetAxis());
            }
            var grip = GetGripOrNull();

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
            bool interactPose = secondaryCurl > 0.7f && inputController.GetTriggerOrNull()?.GetAxis() > 0.7f;

            actions.GrabAction.State = gripPose;
            actions.AbilityGrabAction.State = interactPose;

            var primaryButton = inputController.GetPrimaryButtonOrNull()?.GetPressed();
            var secondaryButton = inputController.GetSecondaryButtonOrNull()?.GetPressed();

            actions.PrimaryAction.State = primaryButton.GetValueOrDefault();
            actions.SecondaryAction.State = secondaryButton.GetValueOrDefault();
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

        public HandPoseData GetHandPose()
        {
            return _handPose;
        }

        public HandActions GetActionsOrNull()
        {
            return _handActions;
        }
    }
}
