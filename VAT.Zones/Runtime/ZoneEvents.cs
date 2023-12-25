using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.Events;

using VAT.Entities;

namespace VAT.Zones
{
    public sealed class ZoneEvents : ZoneComponent
    {
        [Serializable]
        public class ZoneEvent : UnityEvent<EntityIdentifier> { }

        [Serializable]
        public struct ZoneEventGroup
        {
            public ZoneEvent onEntered;

            public ZoneEvent onExited;
        }

        public UnityEvent onZoneEnabled;

        public UnityEvent onZoneDisabled;

        public ZoneEventGroup primaryEvents;

        public ZoneEventGroup secondaryEvents;

        public override void OnZoneEnabled()
        {
            onZoneEnabled.Invoke();
        }

        public override void OnZoneDisabled()
        {
            onZoneDisabled.Invoke();
        }

        public override void OnPrimaryZoneEntered(EntityIdentifier identifier)
        {
            primaryEvents.onEntered.Invoke(identifier);
        }

        public override void OnPrimaryZoneExited(EntityIdentifier identifier)
        {
            primaryEvents.onExited.Invoke(identifier);
        }

        public override void OnSecondaryZoneEntered(EntityIdentifier identifier)
        {
            secondaryEvents.onEntered.Invoke(identifier);
        }

        public override void OnSecondaryZoneExited(EntityIdentifier identifier)
        {
            secondaryEvents.onExited.Invoke(identifier);
        }
    }
}
