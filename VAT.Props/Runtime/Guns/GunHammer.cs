using UnityEngine;

using VAT.Logic;

namespace VAT.Props
{
    public enum HammerState
    {
        RELEASED = 0,
        COCKING = 1,
        COCKED = 2,
    }

    public class GunHammer : Actuator
    {
        [SerializeField]
        private GunBarrel _barrel = null;

        [SerializeField]
        private Threshold _threshold = new();

        private HammerState _state = HammerState.RELEASED;

        public HammerState State => _state;

        public Threshold Threshold => _threshold;

        protected override void OnInputChanged(float value)
        {
            Value = value;

            switch (_threshold.SendInput(value))
            {
                case Threshold.ThresholdPulse.HIGH:
                    Release();
                    break;
                case Threshold.ThresholdPulse.LOW:
                    break;
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
