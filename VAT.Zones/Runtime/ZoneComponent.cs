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

        public virtual void OnPrimaryZoneEntered(EntityIdentifier identifier) { }

        public virtual void OnPrimaryZoneExited(EntityIdentifier identifier) { }

        public virtual void OnSecondaryZoneEntered(EntityIdentifier identifier) { }

        public virtual void OnSecondaryZoneExited(EntityIdentifier identifier) { }
    }
}
