using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Shared.Data;

namespace VAT.Interaction
{
    public class SecondaryGripController : MonoBehaviour, IVirtualControllerOverride
    {
        [SerializeField]
        private Grip _primaryGrip = null;

        [SerializeField]
        private Grip _secondaryGrip = null;

        [SerializeField]
        private InteractableHost _host = null;

        private void Awake()
        {
            _secondaryGrip.DisableInteraction();

            _primaryGrip.AttachCompleteEvent += OnPrimaryAttached;
            _primaryGrip.DetachCompleteEvent += OnPrimaryDetached;

            _secondaryGrip.AttachCompleteEvent += OnSecondaryAttached;
            _secondaryGrip.DetachCompleteEvent += OnSecondaryDetached;

            if (_host == null)
            {
                _host = GetComponentInParent<InteractableHost>();
            }

            _host.VirtualController.RegisterOverride(this);
        }

        private void OnSecondaryAttached(IInteractor interactor)
        {

        }

        private void OnSecondaryDetached(IInteractor interactor)
        {

        }

        private void OnPrimaryAttached(IInteractor interactor)
        {
            _secondaryGrip.EnableInteraction();
        }

        private void OnPrimaryDetached(IInteractor interactor) {
            if (!_primaryGrip.IsHeld)
            {
                IInteractor secondaryInteractor = null;
                if (_secondaryGrip.AttachedInteractors.Count > 0)
                    secondaryInteractor = _secondaryGrip.AttachedInteractors[0];

                _secondaryGrip.DisableInteraction();

                if (secondaryInteractor != null && _primaryGrip.ValidateInteractable(secondaryInteractor).valid)
                {
                    secondaryInteractor.AttachGrip(_primaryGrip);
                }
            }
        }

        public void OnSolveController(VirtualControllerPayload payload)
        {
            if (payload.interactorGripPair.Grip == _secondaryGrip && _primaryGrip.IsHeld)
            {
                var primaryHand = _primaryGrip.GetFirstInteractor();
                var primaryTransform = primaryHand.GetRigidbody().transform;
                var secondaryTransform = payload.interactorGripPair.Interactor.GetRigidbody().transform;

                var relative = SimpleTransform.Create(primaryTransform).InverseTransform(SimpleTransform.Create(secondaryTransform));

                var primaryTarget = primaryHand.GetTargetData().targetInRig;

                var oldTargetInRig = payload.targetInRig.rotation;
                payload.targetInRig = primaryTarget.Transform(relative);
                payload.targetInRig.rotation = oldTargetInRig;
            }
        }
    }
}
