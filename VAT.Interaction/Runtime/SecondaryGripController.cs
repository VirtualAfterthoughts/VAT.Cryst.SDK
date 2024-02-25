using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Shared.Data;

namespace VAT.Interaction
{
    public class SecondaryGripController : VirtualControllerOverride
    {
        [SerializeField]
        private Grip _primaryGrip = null;

        [SerializeField]
        private Grip _secondaryGrip = null;

        private VirtualController _virtualController = new VirtualController();

        public override void Solve(VirtualControllerData data)
        {
            VirtualControllerElement primaryElement = null;
            VirtualControllerElement secondaryElement = null;

            foreach (var element in data.elements)
            {
                if (element.gripPair.Grip == _secondaryGrip)
                {
                    secondaryElement = element;
                }
                else if (element.gripPair.Grip == _primaryGrip)
                {
                    primaryElement = element;
                }
            }

            if (primaryElement != null && secondaryElement != null)
            {
                var primaryGripPair = primaryElement.gripPair;
                var secondaryGripPair = secondaryElement.gripPair;

                var primaryTransform = primaryGripPair.Interactor.GetRigidbody().transform;
                var secondaryTransform = secondaryGripPair.Interactor.GetRigidbody().transform;

                var relative = SimpleTransform.Create(primaryTransform).InverseTransform(SimpleTransform.Create(secondaryTransform));

                var oldTargetInRig = secondaryElement.targetInRig.rotation;
                secondaryElement.targetInRig = primaryElement.targetInRig.Transform(relative);
                secondaryElement.targetInRig.rotation = oldTargetInRig;
            }
        }

        private void Awake()
        {
            _virtualController.RegisterOverride(this);

            _secondaryGrip.DisableInteraction();

            _primaryGrip.AttachCompleteEvent += OnPrimaryAttached;
            _primaryGrip.DetachCompleteEvent += OnPrimaryDetached;

            _secondaryGrip.AttachCompleteEvent += OnSecondaryAttached;
            _secondaryGrip.DetachCompleteEvent += OnSecondaryDetached;
        }

        public void Update()
        {
            _virtualController.Solve();
        }

        private void OnSecondaryAttached(IInteractor interactor)
        {
            _virtualController.RegisterPair(interactor, _secondaryGrip);
        }

        private void OnSecondaryDetached(IInteractor interactor)
        {
            _virtualController.UnregisterPair(interactor);
        }

        private void OnPrimaryAttached(IInteractor interactor)
        {
            _virtualController.RegisterPair(interactor, _primaryGrip);

            _secondaryGrip.EnableInteraction();
        }

        private void OnPrimaryDetached(IInteractor interactor) {
            _virtualController.UnregisterPair(interactor);

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
    }
}
