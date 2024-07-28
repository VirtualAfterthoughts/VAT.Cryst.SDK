using UnityEngine;

using VAT.Cryst.Interfaces;
using VAT.Props.Ammo;

namespace VAT.Props
{
    public class MagWell : MonoBehaviour
    {
        [SerializeField]
        private AmmoSocket _socket = null;

        [SerializeField]
        private Chamber _chamber = null;

        [SerializeField]
        private InterfaceReference<IBolt> _bolt = new();

        private Magazine _peekedMagazine = null;
        private Cartridge _peekedCartridge = null;

        private void OnEnable()
        {
            _bolt.Interface.OnStateChanged += OnBoltStateChanged;
        }

        private void OnDisable()
        {
            _bolt.Interface.OnStateChanged -= OnBoltStateChanged;
        }

        private void OnBoltStateChanged(BoltState previous, BoltState current)
        {
            if (current == BoltState.OPEN)
            {
                _chamber.EjectCartridge();

                if (_socket != null && _socket.LockedPlugs.Count > 0)
                {
                    var plug = _socket.LockedPlugs[0] as AmmoPlug;

                    if (plug != null)
                    {
                        var mag = plug.magazine;

                        if (mag.Cartridges.Count <= 0)
                        {
                            _bolt.Interface.Locked = true;
                            _bolt.Interface.TargetPercent = 1f;
                        }
                    }
                }
            }

            if (current == BoltState.CLOSING)
            {
                if (_socket != null && _socket.LockedPlugs.Count > 0)
                {
                    var plug = _socket.LockedPlugs[0] as AmmoPlug;

                    if (plug != null)
                    {
                        _peekedMagazine = plug.magazine;
                        _peekedCartridge = plug.magazine.PeekCartridge();

                        _chamber.InsertCartridge(_peekedCartridge);
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
        }
    }
}
