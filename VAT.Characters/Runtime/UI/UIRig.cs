using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;
using VAT.UI;
using VAT.Interaction;

namespace VAT.Characters
{
    using VAT.Avatars.Integumentary;
    using VAT.Cryst.Math;

    public class UIRig : CrystRig
    {
        public Transform root;
        public Transform leftWrist;
        public Transform rightWrist;
        public GameObject uiCanvas;
        public XRUIPointer pointer;

        public SpawnUI spawnUi;

        public UIPanelController panelController;

        public override void OnRigEnable()
        {
            base.OnRigEnable();

            uiCanvas.transform.localScale = Vector3.zero;

            var avatarRig = RigManager.GetRigOrNull<AvatarRig>();
            avatarRig.OnSwitchedAvatar += OnSwitchedAvatar;

            if (avatarRig.CurrentAvatar != null)
            {
                OnSwitchedAvatar(avatarRig.CurrentAvatar);
            }
        }

        private void OnSwitchedAvatar(Avatar avatar)
        {
            var avatarRig = RigManager.GetRigOrNull<AvatarRig>();
            if (avatarRig != null)
            {
                var interactors = avatarRig.GetCurrentInteractors();

                foreach (var interactor in interactors)
                {
                    interactor.RegisterModule(spawnUi);
                }
            }
        }

        private float _uiTimer = 0f;

        private bool _wasPressingButton = false;
        private bool _wasHidingButton = false;

        private void GetInputs(IBehaviourRig behaviourRig, Handedness handedness, out bool secondary, out bool trigger)
        {
            behaviourRig.TryGetArm(handedness, out var arm);
            var hand = arm.GetHand();
            var controller = hand.GetInputController();
            var secondaryButton = controller.GetActions()?.SecondaryAction.State;

            secondary = secondaryButton.Value;
            trigger = (controller.GetTrigger()?.GetPressed()).GetValueOrDefault();

            if (handedness == Handedness.LEFT)
            {
                leftWrist.SetLocalPositionAndRotation(hand.Transform.Position, hand.Transform.Rotation);
            }
            else
            {
                rightWrist.SetLocalPositionAndRotation(hand.Transform.Position, hand.Transform.Rotation);
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
                transform.SetPositionAndRotation(root.Position, root.Rotation);
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

            float slerp = Smoothing.CalculateDecay(24f, Time.deltaTime);

            if (_isShown)
            {
                uiCanvas.transform.localScale = Vector3.Slerp(uiCanvas.transform.localScale, Vector3.one, slerp);
            }
            else
            {
                uiCanvas.transform.localScale = Vector3.Slerp(uiCanvas.transform.localScale, Vector3.zero, slerp);
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

                root.position = worldHead.Position;
                root.localPosition = new Vector3(root.localPosition.x, 0f, root.localPosition.z);

                var headForward = worldHead.Forward;
                headForward = transform.InverseTransformDirection(headForward);
                headForward.y = 0f;
                headForward = transform.TransformDirection(headForward);

                root.rotation = Quaternion.LookRotation(headForward, transform.up);
            }

            _isShown = true;

            panelController.Show();
        }

        public void Hide()
        {
            _isShown = false;
        }
    }
}
