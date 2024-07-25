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

    public class GunHammer : MonoBehaviour, IActuatable
    {
        [SerializeField]
        private GunBarrel _barrel = null;

        private HammerState _state = HammerState.RELEASED;

        public HammerState State => _state;

        private bool _isActuated = false;
        public bool IsActuated => _isActuated;

        public void Actuate(bool actuated = true)
        {
            if (actuated == IsActuated)
            {
                return;
            }

            _isActuated = actuated;

            if (actuated)
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
