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

        public abstract void OnZoneEnabled();

        public abstract void OnZoneDisabled();

        public virtual void OnPrimaryZoneEntered(EntityTracker tracker) { }

        public virtual void OnPrimaryZoneExited(EntityTracker tracker) { }

        public virtual void OnSecondaryZoneEntered(EntityTracker tracker) { }

        public virtual void OnSecondaryZoneExited(EntityTracker tracker) { }

#if UNITY_EDITOR
        public virtual void OnEditorZoneEnabled() { }

        public virtual void OnEditorZoneDisabled() { }
#endif
    }
}
