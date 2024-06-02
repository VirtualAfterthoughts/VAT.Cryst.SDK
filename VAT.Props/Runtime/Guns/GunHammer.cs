using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Props
{
    public enum HammerState
    {
        RELEASED = 0,
        COCKING = 1,
        COCKED = 2,
    }

    public class GunHammer : MonoBehaviour
    {
        [SerializeField]
        private GunBarrel _barrel = null;

        private HammerState _state = HammerState.RELEASED;

        public HammerState State => _state;

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
