using System;
using System.Collections.Generic;

using UnityEngine;

using VAT.Interaction.Entities;
using VAT.Packaging;

namespace VAT.Interaction.Attachments
{
    public abstract class Socket : MonoBehaviour
    {
        [SerializeField]
        private CrystBody _body = null;

        [SerializeField]
        private TagMask _tagMask = new();

        private readonly List<Plug> _registeredPlugs = new();
        private readonly List<Plug> _lockedPlugs = new();

        public CrystBody Body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
            }
        }

        public TagMask TagMask => _tagMask;

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

            return TagMask.HasTag(plug.Tag);
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
