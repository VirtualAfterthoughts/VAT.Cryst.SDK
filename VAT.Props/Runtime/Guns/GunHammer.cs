using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Cryst;

namespace VAT.Props
{
    public enum HammerState
    {
        RELEASED = 0,
        COCKING = 1,
        COCKED = 2,
    }

    public class GunHammer : Actuatable
    {
        [SerializeField]
        private GunBarrel _barrel = null;

        private HammerState _state = HammerState.RELEASED;

        public HammerState State => _state;

        protected override void OnActuated(bool isActuated)
        {
            if (isActuated)
            {
                Release();
            }
        }

        public void Cock()
        {
            _state = HammerState.COCKED;
        }

        public void Release()
        {
            if (_state != HammerState.COCKED)
            {
                return;
            }

            _state = HammerState.RELEASED;

            _barrel.Fire();
        }
    }
}
