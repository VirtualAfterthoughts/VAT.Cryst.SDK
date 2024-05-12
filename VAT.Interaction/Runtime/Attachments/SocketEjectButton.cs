using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Interaction.Attachments
{
    public class SocketEjectButton : MonoBehaviour
    {
        [SerializeField]
        private Grip _grip = null;

        [SerializeField]
        private Socket _socket = null;

        private IInteractor _mainInteractor = null;

        private void OnEnable()
        {
            _grip.OnAttached += OnAttached;
            _grip.OnDetached += OnDetached;
        }

        private void OnDisable()
        {
            _grip.OnAttached -= OnAttached;
            _grip.OnDetached -= OnDetached;
        }

        private void OnAttached(IInteractor interactor)
        {
            if (_mainInteractor == null)
            {
                _mainInteractor = interactor;
                interactor.GetInputHand().GetInputController().GetActions().SecondaryAction.OnStateChanged += OnSecondaryActionChanged;
            }
        }

        private void OnDetached(IInteractor interactor)
        {
            if (_mainInteractor == interactor)
            {
                _mainInteractor = null;
                interactor.GetInputHand().GetInputController().GetActions().SecondaryAction.OnStateChanged -= OnSecondaryActionChanged;
            }
        }

        private void OnSecondaryActionChanged(bool value)
        {
            if (value)
            {
                _socket.EjectPlugs();
            }
        }
    }
}
