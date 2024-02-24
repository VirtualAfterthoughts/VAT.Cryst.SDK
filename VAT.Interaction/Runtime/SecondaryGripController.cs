using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Interaction
{
    public class SecondaryGripController : MonoBehaviour
    {
        [SerializeField]
        private Grip _primaryGrip = null;

        [SerializeField]
        private Grip _secondaryGrip = null;

        private void Awake()
        {
            _secondaryGrip.DisableInteraction();

            _primaryGrip.AttachCompleteEvent += OnPrimaryAttached;
            _primaryGrip.DetachCompleteEvent += OnPrimaryDetached;
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
    }
}
