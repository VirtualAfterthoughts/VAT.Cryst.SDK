using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Interaction;

namespace VAT.Props
{
    public class GunTrigger : MonoBehaviour
    {
        [SerializeField]
        private Gun _gun = null;

        [SerializeField]
        private Grip[] _triggerGrips = new Grip[0];

        [SerializeField]
        private GunHammer _hammer = null;

        private IInteractor _triggerInteractor = null;

        private const float ACTUATION_THRESHOLD = 0.8f;

        private bool _isActuated = false;

        private void OnEnable()
        {
            foreach (var grip in _triggerGrips)
            {
                grip.OnAttached += OnTriggerAttached;
                grip.OnDetached += OnTriggerDetached;
            }
        }

        private void OnDisable()
        {
            foreach (var grip in _triggerGrips)
            {
                grip.OnAttached -= OnTriggerAttached;
                grip.OnDetached -= OnTriggerDetached;
            }
        }

        private void OnTriggerAttached(IInteractor interactor)
        {
            if (_triggerInteractor == null)
            {
                _triggerInteractor = interactor;
            }
        }

        private void OnTriggerDetached(IInteractor interactor)
        {
            if (_triggerInteractor == interactor)
            {
                _triggerInteractor = null;
            }
        }

        private void Update()
        {
            if (_triggerInteractor == null)
            {
                return;
            }

            var trigger = _triggerInteractor.GetInputHand().GetInputController().GetTrigger();
            var axis = trigger?.GetAxis();

            bool actuated = axis > ACTUATION_THRESHOLD;
            if (actuated != _isActuated)
            {
                _isActuated = actuated;
                
                if (actuated)
                {
                    _hammer.Release();
                }
            }
        }
    }
}
