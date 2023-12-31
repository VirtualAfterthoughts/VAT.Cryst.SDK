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
        public class ZoneEvent : UnityEvent<EntityTracker> { }

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

        public override void OnPrimaryZoneEntered(EntityTracker tracker)
        {
            primaryEvents.onEntered.Invoke(tracker);
        }

        public override void OnPrimaryZoneExited(EntityTracker tracker)
        {
            primaryEvents.onExited.Invoke(tracker);
        }

        public override void OnSecondaryZoneEntered(EntityTracker tracker)
        {
            secondaryEvents.onEntered.Invoke(tracker);
        }

        public override void OnSecondaryZoneExited(EntityTracker tracker)
        {
            secondaryEvents.onExited.Invoke(tracker);
        }
    }
}
