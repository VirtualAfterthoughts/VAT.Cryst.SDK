using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Props
{
    public class Cartridge : MonoBehaviour
    {
        [SerializeField]
        private CartridgeDataReference _data;

        public CartridgeDataReference Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }
}
