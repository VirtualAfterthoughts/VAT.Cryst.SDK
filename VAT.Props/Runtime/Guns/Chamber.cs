using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Props
{
    public class Chamber : MonoBehaviour
    {
        [SerializeField]
        private Transform _cartridgeTarget = null;

        public Transform CartridgeTarget
        {
            get
            {
                if (_cartridgeTarget == null)
                {
                    _cartridgeTarget = transform;
                }

                return _cartridgeTarget;
            }
        }

        private Cartridge _cartridge = null;
        public Cartridge Cartridge => _cartridge;

        public bool InsertCartridge(Cartridge cartridge)
        {
            if (Cartridge != null)
            {
                return false;
            }

            if (cartridge == null)
            {
                return false;
            }

            _cartridge = cartridge;

            cartridge.transform.parent = CartridgeTarget;
            cartridge.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            return true;
        }

        public Cartridge TakeCartridge()
        {
            if (Cartridge == null)
            {
                return null;
            }

            var takenCartridge = Cartridge;
            _cartridge = null;

            takenCartridge.transform.parent = null;

            return takenCartridge;
        }
    }
}
