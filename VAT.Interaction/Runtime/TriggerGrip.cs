using UnityEngine;

using VAT.Cryst.Game;
using VAT.Cryst.Interfaces;
using VAT.Logic;

namespace VAT.Interaction
{
    public class TriggerGrip : Node, IUpdateable
    {
        [SerializeField]
        [Tooltip("The grip used for this trigger.")]
        private Grip _grip = null;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The threshold required for the trigger to actuate. 0 means not pressed at all, 0.5 means halfway, and 1 means fully pressed. It is recommended to have a little bit of leeway, so the default is 0.8.")]
        private float _actuationThreshold = 0.8f;

        public Grip Grip { get { return _grip; } set { _grip = value; } }

        public float ActuationThreshold { get { return _actuationThreshold; } set { _actuationThreshold = value; } }

        private IInteractor _triggerInteractor = null;

        private bool _isActuated = false;

        private void OnEnable()
        {
            _grip.OnAttached += OnTriggerAttached;
            _grip.OnDetached += OnTriggerDetached;
        }

        private void OnDisable()
        {
            _grip.OnAttached -= OnTriggerAttached;
            _grip.OnDetached -= OnTriggerDetached;

            CrystUpdateManager.UnregisterUpdatable(this);
        }

        private void OnTriggerAttached(IInteractor interactor)
        {
            _triggerInteractor ??= interactor;

            CrystUpdateManager.RegisterUpdatable(this);
        }

        private void OnTriggerDetached(IInteractor interactor)
        {
            if (_triggerInteractor == interactor)
            {
                _triggerInteractor = null;

                CrystUpdateManager.UnregisterUpdatable(this);

                Actuate(false);
            }
        }

        public void OnUpdate(float deltaTime)
        {
            var trigger = _triggerInteractor.GetInputHand().GetInputController().GetTrigger();
            var axis = trigger?.GetAxis();

            bool actuated = axis > ActuationThreshold;

            if (actuated != _isActuated)
            {
                Actuate(actuated);
            }
        }

        private void Actuate(bool isActuated)
        {
            Value = isActuated ? 1f : 0f;

            _isActuated = isActuated;
        }
    }
}
