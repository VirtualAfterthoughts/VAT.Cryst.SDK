using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Props
{
    public class GunBarrel : MonoBehaviour
    {
        [SerializeField]
        private Transform _firePoint = null;

        public event Action OnFire;

        public SimpleTransform GetFirePoint()
        {
            return SimpleTransform.Create(_firePoint.position, _firePoint.rotation);
        }

        public void Fire()
        {
            OnFire?.Invoke();
        }
    }
}
