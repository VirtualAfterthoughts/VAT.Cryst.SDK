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

        private float _smoothAxis = 0f;
        private float _smoothVelocity = 0f;

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

            crouchAxis = Mathf.Clamp01((Mathf.Abs(crouchAxis) - 0.5f) * 2f) * Mathf.Sign(crouchAxis);
            _smoothAxis = Mathf.SmoothDamp(_smoothAxis, crouchAxis, ref _smoothVelocity, 0.1f);

            var behaviourSpace = _behaviourRig.GetBehaviourSpace();

            float tipToeMult = 1.05f;

            if (_enabled && Mathf.Abs(_smoothAxis) > 0.01f)
            {
                float crouchDelta = _smoothAxis * Time.deltaTime * 2f * behaviourSpace.scale.y;

                behaviourSpace.position += behaviourSpace.up * crouchDelta;

                tipToeMult = 1f;
            }

            float playerHeight = behaviourSpace.scale.y * _playerHeight;

            float headHeight = playerHeight * BodyMeasurementHelper.HeadHeightPercent * 0.5f;

            float headPos = GetHeadY(headHeight);

            float clampedPos = Mathf.Clamp(headPos, 0f, playerHeight * tipToeMult);

            behaviourSpace.position += behaviourSpace.up * (clampedPos - headPos);

            _behaviourRig.SetBehaviourSpace(behaviourSpace);
        }

        public float GetHeadY(float headHeight)
        {
            var localHead = _behaviourRig.GetLocalHead();
            float headPos = (localHead.position - localHead.forward * headHeight + localHead.up * headHeight).y;

            return headPos;
        }

        public void AutoCalculateOffset()
        {
            var behaviourSpace = _behaviourRig.GetBehaviourSpace();

            float playerHeight = _playerHeight * behaviourSpace.scale.y;

            float headHeight = playerHeight * BodyMeasurementHelper.HeadHeightPercent * 0.5f;

            float headPos = GetHeadY(headHeight);

            behaviourSpace.position += behaviourSpace.up * (playerHeight - headPos);

            _behaviourRig.SetBehaviourSpace(behaviourSpace);
        }
    }
}
