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
                    return transform;
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

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.matrix = CartridgeTarget.localToWorldMatrix;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 0.02f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(Vector3.zero, 0.01f);
        }
#endif
    }
}
