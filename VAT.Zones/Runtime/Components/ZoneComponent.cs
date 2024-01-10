using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Entities;

namespace VAT.Zones
{
    public abstract class ZoneComponent : MonoBehaviour
    {
        [SerializeField]
        private ZoneState _triggerFlags = (ZoneState)~0;

        public ZoneState TriggerFlags => _triggerFlags;

        public virtual void OnZoneEnabled() { }

        public virtual void OnZoneDisabled() { }

        public virtual void OnEntityEnter(CrystEntityTracker tracker) { }

        public virtual void OnEntityExit(CrystEntityTracker tracker) { }

        public virtual void OnPrimaryZoneEntered(CrystEntityTracker tracker) { }

        public virtual void OnPrimaryZoneExited(CrystEntityTracker tracker) { }

        public virtual void OnSecondaryZoneEntered(CrystEntityTracker tracker) { }

        public virtual void OnSecondaryZoneExited(CrystEntityTracker tracker) { }

#if UNITY_EDITOR
        public virtual void OnEditorZoneEnabled() { }

        public virtual void OnEditorZoneDisabled() { }
#endif
    }
}
