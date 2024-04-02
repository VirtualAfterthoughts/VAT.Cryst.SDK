using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Input.Data;

namespace VAT.Characters
{
    [Serializable]
    public class CrouchBehaviour : ISubBehaviour
    {
        [SerializeField]
        private bool _enabled = true;

        private IBehaviourRig _behaviourRig = null;

        private float _playerHeight = 1.76f;

        public void OnRegister(IBehaviourRig behaviourRig)
        {
            _behaviourRig = behaviourRig;

            var vitals = behaviourRig.RigManager?.GetVitalsOrNull();

            if (vitals != null)
            {
                vitals.OnUpdatedVitals += OnUpdatedVitals;

                OnUpdatedVitals(vitals);
            }
        }


        public void OnDeregister(IBehaviourRig behaviourRig)
        {
            var vitals = behaviourRig.RigManager?.GetVitalsOrNull();

            if (vitals != null)
            {
                vitals.OnUpdatedVitals -= OnUpdatedVitals;

                _playerHeight = 1.76f;
            }

            _behaviourRig = null;
        }

        private void OnUpdatedVitals(ICrystVitals vitals)
        {
            _playerHeight = vitals.PlayerMeasurements.height;

            AutoCalculateOffset();
        }

        public void Solve()
        {
            var hand = _behaviourRig.GetPrimaryHand();
            var controller = hand.GetInputControllerOrNull();

            if (controller == null)
            {
                return;
            }

            var thumbstick = controller.GetThumbstickOrNull();

            if (thumbstick == null)
            {
                return;
            }

            float crouchAxis = thumbstick.GetAxis().y;

            var behaviourSpace = _behaviourRig.GetBehaviourSpace();

            if (_enabled && Mathf.Abs(crouchAxis) > 0.1f)
            {
                float crouchDelta = crouchAxis * Time.deltaTime * 2f;

                behaviourSpace.position += behaviourSpace.up * crouchDelta;
            }

            float headHeight = _playerHeight * BodyMeasurementHelper.HeadHeightPercent * 0.5f;

            var localHead = _behaviourRig.GetLocalHead();
            float headPos = (localHead.position + localHead.up * headHeight).y;
            float clampedPos = Mathf.Clamp(headPos, 0f, _playerHeight);

            behaviourSpace.position += behaviourSpace.up * (clampedPos - headPos);

            _behaviourRig.SetBehaviourSpace(behaviourSpace);
        }

        public void AutoCalculateOffset()
        {
            var behaviourSpace = _behaviourRig.GetBehaviourSpace();

            float headHeight = _playerHeight * BodyMeasurementHelper.HeadHeightPercent * 0.5f;

            float headPos = _behaviourRig.GetLocalHead().position.y + headHeight;

            behaviourSpace.position += behaviourSpace.up * (_playerHeight - headPos);

            _behaviourRig.SetBehaviourSpace(behaviourSpace);
        }
    }
}
