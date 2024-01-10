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
        public class ZoneEvent : UnityEvent<CrystEntityTracker> { }

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

        public override void OnPrimaryZoneEntered(CrystEntityTracker tracker)
        {
            primaryEvents.onEntered.Invoke(tracker);
        }

        public override void OnPrimaryZoneExited(CrystEntityTracker tracker)
        {
            primaryEvents.onExited.Invoke(tracker);
        }

        public override void OnSecondaryZoneEntered(CrystEntityTracker tracker)
        {
            secondaryEvents.onEntered.Invoke(tracker);
        }

        public override void OnSecondaryZoneExited(CrystEntityTracker tracker)
        {
            secondaryEvents.onExited.Invoke(tracker);
        }
    }
}
