using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Props
{
    public class Chamber : MonoBehaviour
    {
        private Cartridge _cartridge = null;
        public Cartridge Cartridge => _cartridge;

        public bool InsertCartridge(Cartridge cartridge)
        {
            if (Cartridge != null)
            {
                return false;
            }

            _cartridge = cartridge;

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

            return takenCartridge;
        }
    }
}
