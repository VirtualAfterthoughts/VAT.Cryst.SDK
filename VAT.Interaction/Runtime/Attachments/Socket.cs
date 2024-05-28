using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction.Attachments
{
    public abstract class Socket : MonoBehaviour
    {
        [SerializeField]
        private InteractableHost _interactableHost = null;

        [SerializeField]
        private SlotShapeReference _slotShape = new();

        private readonly List<Plug> _registeredPlugs = new();
        private readonly List<Plug> _lockedPlugs = new();

        public InteractableHost Host
        {
            get
            {
                return _interactableHost;
            }
            set
            {
                _interactableHost = value;
            }
        }

        public SlotShapeReference SlotShape => _slotShape;

        public List<Plug> RegisteredPlugs => _registeredPlugs;

        public List<Plug> LockedPlugs => _lockedPlugs;

        public virtual bool IsLocked => RegisteredPlugs.Count > 0;

        public event Action<Plug> OnRegisterPlug, OnUnregisterPlug, OnLockPlug, OnUnlockPlug;

        public void EjectPlugs()
        {
            foreach (var plug in RegisteredPlugs)
            {
                if (!plug.IsLocked)
                {
                    continue;
                }

                plug.ConfirmEject();
            }
        }

        public bool IsPlugLocked(Plug plug)
        {
            return LockedPlugs.Contains(plug);
        }

        public bool HasPlug(Plug plug)
        {
            return RegisteredPlugs.Contains(plug);
        }

        public bool CanInsert(Plug plug)
        {
            if (IsLocked)
            {
                return false;
            }

            SlotShape.TryGetShard(out var socketShape);
            plug.SlotShape.TryGetShard(out var plugShape);

            if (socketShape == null || plugShape == null)
            {
                return false;
            }

            return socketShape.MatchesShape(plugShape);
        }

        public void RegisterPlug(Plug plug)
        {
            _registeredPlugs.Add(plug);

            OnRegisterPlug?.Invoke(plug);
        }

        public void UnregisterPlug(Plug plug)
        {
            _registeredPlugs.Remove(plug);

            OnUnregisterPlug?.Invoke(plug);
        }

        public void LockPlug(Plug plug)
        {
            _lockedPlugs.Add(plug);

            OnLockPlug?.Invoke(plug);
        }

        public void UnlockPlug(Plug plug)
        {
            _lockedPlugs.Remove(plug);

            OnUnlockPlug?.Invoke(plug);
        }
    }
}
