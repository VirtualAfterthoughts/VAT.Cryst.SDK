using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Input;
using VAT.Input.UI;
using VAT.Shared.Extensions;

namespace VAT.Characters
{
    public class UIRig : CrystRig
    {
        public Transform root;
        public Transform leftWrist;
        public Transform rightWrist;
        public GameObject uiCanvas;
        public XRUIPointer pointer;

        public override void OnRigEnable()
        {
            base.OnRigEnable();

            uiCanvas.transform.localScale = Vector3.zero;
        }

        private float _uiTimer = 0f;

        private bool _wasPressingButton = false;
        private bool _wasHidingButton = false;

        private void GetInputs(IBehaviourRig behaviourRig, Handedness handedness, out bool secondary, out bool trigger)
        {
            behaviourRig.TryGetArm(handedness, out var arm);
            var hand = arm.GetHandOrNull();
            var controller = hand.GetInputControllerOrNull();
            var secondaryButton = controller.GetActionsOrNull()?.SecondaryAction.State;

            secondary = secondaryButton.Value;
            trigger = (controller.GetTriggerOrNull()?.GetPressed()).GetValueOrDefault();

            if (handedness == Handedness.LEFT)
            {
                leftWrist.SetLocalPositionAndRotation(hand.Transform.position, hand.Transform.rotation);
            }
            else
            {
                rightWrist.SetLocalPositionAndRotation(hand.Transform.position, hand.Transform.rotation);
            }
        }

        private Handedness _currentHandedness = Handedness.RIGHT;

        private void SwitchHandedness(Handedness handedness)
        {
            if (handedness == _currentHandedness)
            {
                return;
            }

            if (handedness == Handedness.LEFT)
            {
                pointer.transform.SetParent(leftWrist, false);
            }
            else
            {
                pointer.transform.SetParent(rightWrist, false);
            }

            _currentHandedness = handedness;
        }

        public override void OnLateUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);

            var behaviourRig = RigManager.GetRigOrNull<IBehaviourRig>();
            if (behaviourRig != null)
            {
                var root = behaviourRig.GetRoot();
                transform.SetPositionAndRotation(root.position, root.rotation);
            }

            GetInputs(behaviourRig, Handedness.LEFT, out var leftSecondary, out var leftTrigger);
            GetInputs(behaviourRig, Handedness.RIGHT, out var rightSecondary, out var rightTrigger);

            if (leftTrigger)
            {
                SwitchHandedness(Handedness.LEFT);
            }
            else if (rightTrigger)
            {
                SwitchHandedness(Handedness.RIGHT);
            }

            bool activeTrigger = _currentHandedness == Handedness.LEFT ? leftTrigger : rightTrigger;

            pointer.SetPressed(activeTrigger);

            bool pressingSecondary = leftSecondary || rightSecondary;

            if (!pressingSecondary)
            {
                _wasHidingButton = false;
            }

            if (_isShown && pressingSecondary && !_wasPressingButton)
            {
                Hide();
                _uiTimer = 0f;
                _wasHidingButton = true;
            }
            else if (!_isShown && pressingSecondary && !_wasHidingButton)
            {
                _uiTimer += Time.deltaTime;
            }
            else
            {
                _uiTimer = 0f;
            }

            if (_uiTimer > 0.2f && !_isShown)
            {
                Show();
            }

            _wasPressingButton = pressingSecondary;

            if (_isShown)
            {
                uiCanvas.transform.localScale = Vector3.Slerp(uiCanvas.transform.localScale, Vector3.one, Time.deltaTime * 24f);
            }
            else
            {
                uiCanvas.transform.localScale = Vector3.Slerp(uiCanvas.transform.localScale, Vector3.zero, Time.deltaTime * 24f);
            }
        }

        private bool _isShown = false;

        public void Show()
        {
            var behaviourRig = RigManager.GetRigOrNull<IBehaviourRig>();
            if (behaviourRig != null)
            {
                behaviourRig.TryGetHead(out var head);
                var behaviourRoot = behaviourRig.GetRoot();
                var worldHead = behaviourRoot.Transform(head.Transform);

                root.position = worldHead.position;
                root.localPosition = new Vector3(root.localPosition.x, 0f, root.localPosition.z);

                var headForward = worldHead.forward;
                headForward.y = 0f;

                root.rotation = Quaternion.LookRotation(headForward, transform.up);
            }

            _isShown = true;
        }

        public void Hide()
        {
            _isShown = false;
        }
    }
}
