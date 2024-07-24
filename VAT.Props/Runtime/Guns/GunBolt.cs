using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Props.Ammo;

namespace VAT.Props
{
    public enum BoltState
    {
        CLOSED = 0,
        OPENING = 1,
        OPEN = 2,
        CLOSING = 3,
    }

    public delegate void BoltCallback(BoltState previous, BoltState current);

    public class GunBolt : MonoBehaviour
    {
        public AmmoSocket socket;
        public Chamber chamber;

        private float _pulledPercent = 0f;
        public float PulledPercent => _pulledPercent;

        private bool _isLocked = false;
        public bool IsLocked => _isLocked;

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

        public void UpdateBolt(float percent)
        {
            _pulledPercent = percent;

            UpdateState(percent);
        }

        private Magazine _peekedMagazine = null;
        private Cartridge _peekedCartridge = null;

        private void OnBoltStateChanged(BoltState previous, BoltState current)
        {
            if (current == BoltState.OPEN)
            {
                chamber.EjectCartridge();
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

        public void Lock()
        {
            UpdateBolt(1f);
            _isLocked = true;
        }

        public void Unlock()
        {
            _isLocked = false;
        }
    }
}
