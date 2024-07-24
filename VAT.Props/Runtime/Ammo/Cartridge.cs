using UnityEngine;

using VAT.Interaction.Entities;

namespace VAT.Props
{
    public class Cartridge : MonoBehaviour
    {
        [SerializeField]
        private CrystEntity _entity = null;

        [SerializeField]
        private CartridgeDataReference _data = null;

        public CrystEntity Entity
        {
            get
            {
                return _entity;
            }
            set
            {
                _entity = value;
            }
        }

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

        private bool _spent = false;

        public bool Spent => _spent;
    }
}
