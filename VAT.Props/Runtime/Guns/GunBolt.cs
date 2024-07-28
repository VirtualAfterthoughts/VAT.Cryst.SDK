using UnityEngine;

using VAT.Props.Ammo;

namespace VAT.Props
{
    public class GunBolt : MonoBehaviour, IBolt
    {
        public AmmoSocket socket;
        public Chamber chamber;

        private float _openedPercent = 0f;
        public float OpenedPercent => _openedPercent;

        private float _openedVelocity = 0f;

        private float _targetPercent = 0f;
        public float TargetPercent { 
            get 
            { 
                return _targetPercent;
            } 
            set 
            { 
                _targetPercent = value;

                if (Overriden)
                {
                    _openedPercent = value;
                    UpdateState(OpenedPercent);
                }
            } 
        }

        private bool _locked = false;
        public bool Locked { get { return _locked; } set { _locked = value; } }

        private bool _overriden = false;
        public bool Overriden { get { return _overriden; } set { _overriden = value; } }

        [SerializeField]
        private BoltState _state = BoltState.CLOSED;
        public BoltState State
        {
            get 
            { 
                return _state; 
            }
            set
            {
                if (_state == value)
                {
                    return;
                }

                var previousState = _state;
                _state = value;

                OnBoltStateChanged(previousState, _state);
            }
        }

        public event BoltCallback OnStateChanged;

        public void ResetTarget()
        {
            TargetPercent = 0f;
        }

        private void LateUpdate()
        {
            ApplySpring();

            UpdateState(OpenedPercent);
        }

        private void ApplySpring()
        {
            _openedPercent = Mathf.SmoothDamp(_openedPercent, _targetPercent, ref _openedVelocity, 0.01f);
        }

        private Magazine _peekedMagazine = null;
        private Cartridge _peekedCartridge = null;

        private void OnBoltStateChanged(BoltState previous, BoltState current)
        {
            if (current == BoltState.OPEN)
            {
                chamber.EjectCartridge();

                if (socket != null && socket.LockedPlugs.Count > 0)
                {
                    var plug = socket.LockedPlugs[0] as AmmoPlug;

                    if (plug != null)
                    {
                        var mag = plug.magazine;
                        
                        if (mag.Cartridges.Count <= 0)
                        {
                            Locked = true;
                            TargetPercent = 1f;
                        }
                    }
                }
            }

            if (current == BoltState.CLOSING)
            {
                if (socket != null && socket.LockedPlugs.Count > 0)
                {
                    var plug = socket.LockedPlugs[0] as AmmoPlug;

                    if (plug != null)
                    {
                        _peekedMagazine = plug.magazine;
                        _peekedCartridge = plug.magazine.PeekCartridge();

                        chamber.InsertCartridge(_peekedCartridge);
                    }
                }
            }

            if (current == BoltState.CLOSED)
            {
                if (_peekedMagazine != null && _peekedCartridge != null)
                {
                    var parent = _peekedCartridge.transform.parent;

                    _peekedMagazine.UnloadCartridge(_peekedCartridge);

                    foreach (var body in _peekedCartridge.Entity.Bodies)
                    {
                        body.Freeze(true);
                        body.Rigidbody.detectCollisions = false;
                    }

                    _peekedCartridge.transform.parent = parent;

                    _peekedMagazine = null;
                    _peekedCartridge = null;
                }
            }

            OnStateChanged?.Invoke(previous, current);
        }

        private void UpdateState(float percent)
        {
            switch (State) 
            {
                case BoltState.CLOSED:
                    if (percent >= 0.01f)
                    {
                        State = BoltState.OPENING;
                        UpdateState(percent);
                    }
                    break;
                case BoltState.OPENING:
                    if (percent >= 0.99f)
                    {
                        State = BoltState.OPEN;
                        UpdateState(percent);
                    }
                    else if (percent < 0.01f)
                    {
                        State = BoltState.CLOSED;
                        UpdateState(percent);
                    }
                    break;
                case BoltState.OPEN:
                    if (percent < 0.99f)
                    {
                        State = BoltState.CLOSING;
                        UpdateState(percent);
                    }
                    break;
                case BoltState.CLOSING:
                    if (percent < 0.01f)
                    {
                        State = BoltState.CLOSED;
                        UpdateState(percent);
                    }
                    break;
            }
        }
    }
}
