using System;

namespace VAT.Cryst.Interfaces
{
    using UnityEngine;

    [Serializable]
    public class InterfaceReference<TInterface> where TInterface : class
    {
        [SerializeField]
        private Object _target = null;

        public Object Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }

        public TInterface Interface
        {
            get
            {
                return _target as TInterface;
            }
        }
    }
}
