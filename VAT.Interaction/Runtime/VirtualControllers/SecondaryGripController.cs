using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public sealed class SecondaryGripController : MonoBehaviour, IVirtualControllerOverride
    {
        [Header("References")]
        [SerializeField]
        [Tooltip("The interactable host that this is a part of.")]
        private InteractableHost _host = null;

        [SerializeField]
        [Tooltip("The primary grip.")]
        private Grip _primaryGrip = null;

        [SerializeField]
        [Tooltip("All secondary grips that will be enabled when the primary grip is grabbed.")]
        private Grip[] _secondaryGrips = new Grip[0];

        [Header("Options")]
        [SerializeField]
        [Tooltip("If true, the hand attached to the secondary grip will swap to the primary grip when it is detached.")]
        private bool _swapHandOnDetach = true;

        private void OnEnable()
        {
            foreach (var grip in _secondaryGrips)
            {
                grip.DisableInteraction();
            }

            _primaryGrip.AttachCompleteEvent += OnPrimaryGripAttached;
            _primaryGrip.DetachCompleteEvent += OnPrimaryGripDetached;

            if (_host == null)
            {
                _host = GetComponentInParent<InteractableHost>();
            }

            _host.VirtualController.RegisterOverride(this);
        }

        private void OnDisable()
        {
            _primaryGrip.AttachCompleteEvent -= OnPrimaryGripAttached;
            _primaryGrip.DetachCompleteEvent -= OnPrimaryGripDetached;

            if (_host != null)
            {
                _host.VirtualController.UnregisterOverride(this);
            }

            _host = null;
        }

        private void OnPrimaryGripAttached(IInteractor interactor)
        {
            ToggleSecondaryGrips(true);
        }

        private void ToggleSecondaryGrips(bool enabled)
        {
            foreach (var grip in _secondaryGrips)
            {
                if (enabled)
                {
                    grip.EnableInteraction();
                }
                else
                {
                    grip.DisableInteraction();
                }
            }
        }

        private IInteractor GetSecondaryInteractor()
        {
            if (_secondaryGrips.Length > 0)
            {
                return _secondaryGrips[0].GetFirstInteractor();
            }

            return null;
        }

        private void SwapInteractor()
        {
            var secondaryInteractor = GetSecondaryInteractor();

            if (secondaryInteractor != null && _primaryGrip.ValidateInteractable(secondaryInteractor).valid)
            {
                secondaryInteractor.DetachGrips();
                secondaryInteractor.AttachGrip(_primaryGrip);
            }
        }

        private void OnPrimaryGripDetached(IInteractor interactor) {
            if (!_primaryGrip.IsHeld)
            {
                if (_swapHandOnDetach)
                {
                    SwapInteractor();
                }

                ToggleSecondaryGrips(false);
            }
        }

        public void OnSolveController(VirtualControllerPayload payload)
        {
            if (_secondaryGrips.Contains(payload.ActivePair.Grip) && _primaryGrip.IsHeld)
            {
                var primaryHand = _primaryGrip.GetFirstInteractor();
                var primaryTransform = primaryHand.GetRigidbody().transform;

                var grabberPoint = payload.ActivePair.Interactor.GetGrabberPoint();

                var secondaryTransform = payload.ActivePair.Interactor.GetRigidbody().transform;
                var secondaryGrabTransform = grabberPoint.GetParentTransform().Transform(payload.ActivePair.Grip.GetTargetInInteractor(grabberPoint));
                var grabTarget = payload.ActivePair.Grip.GetTargetInWorld(grabberPoint);
                var relativeToGrab = secondaryGrabTransform.InverseTransform(SimpleTransform.Create(secondaryTransform));

                var relative = SimpleTransform.Create(primaryTransform).InverseTransform(grabTarget);

                var primaryTarget = primaryHand.GetTargetData().targetInRig;

                var oldTargetInRig = payload.TargetInRig.rotation;
                var result = primaryTarget.Transform(relative).Transform(relativeToGrab);
                result.rotation = oldTargetInRig;
                payload.TargetInRig = result;
            }
        }
    }
}
