using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Props
{
    public class Gun : MonoBehaviour
    {
        [SerializeField]
        private float _roundsPerMinute = 700f;

        private float SecondsPerShot => 60f / _roundsPerMinute;

        private bool _isActuated = false;

        public void Fire()
        {
            Debug.Log("FIRE!");
        }

        public void SetActuation(bool actuated)
        {
            if (_isActuated == actuated)
            {
                return;
            }

            OnActuationChanged(actuated);
        }

        private void OnActuationChanged(bool actuated)
        {
            _isActuated = actuated;
        }

        private void FixedUpdate()
        {
            if (_isActuated)
            {

            }
        }
    }
}
