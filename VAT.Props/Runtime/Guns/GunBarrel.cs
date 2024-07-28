using System;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Props
{
    public class GunBarrel : MonoBehaviour
    {
        [SerializeField]
        private Transform _firePoint = null;

        [SerializeField]
        private Chamber _chamber = null;

        public event Action OnFire;

        public Chamber Chamber
        {
            get
            {
                return _chamber;
            }
            set
            {
                _chamber = value;
            }
        }

        public SimpleTransform GetFirePoint()
        {
            return SimpleTransform.Create(_firePoint.position, _firePoint.rotation);
        }

        public void Fire()
        {
            if (Chamber.Cartridge != null && !Chamber.Cartridge.Spent)
            {
                OnFire?.Invoke();
            }
        }
    }
}
