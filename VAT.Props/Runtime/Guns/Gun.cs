using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Props
{
    public class Gun : MonoBehaviour
    {
        [SerializeField]
        private float _roundsPerMinute = 700f;

        [SerializeField]
        private GunBarrel _barrel = null;

        private float SecondsPerShot => 60f / _roundsPerMinute;

        private bool _isActuated = false;

        private float _timeSinceLastFire = 0f;

        public void Fire()
        {
            var force = Vector3.back * 70f;
            GetComponent<Rigidbody>().AddRelativeForce(force, ForceMode.Impulse);
            GetComponent<Rigidbody>().AddRelativeTorque(force * 70f, ForceMode.Impulse);
            _barrel.Fire();
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

        private bool _wasActuated = false;

        private void FixedUpdate()
        {
            //_timeSinceLastFire += Time.deltaTime;
            //
            //if (_isActuated && !_wasActuated)
            //{
            //    if (_timeSinceLastFire >= SecondsPerShot)
            //    {
            //        Fire();
            //
            //        _timeSinceLastFire = 0f;
            //    }
            //}
            //
            //_wasActuated = _isActuated;
        }
    }
}
