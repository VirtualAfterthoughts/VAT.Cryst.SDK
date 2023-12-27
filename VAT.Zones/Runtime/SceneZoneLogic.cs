using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Entities;

using VAT.Shared;

namespace VAT.Zones
{
    public partial class SceneZone : CachedMonoBehaviour
    {
        private readonly List<EntityTracker> _primaryEntities = new();
        private readonly Dictionary<EntityTracker, int> _secondaryEntities = new();

        private ZoneState _state;
        public ZoneState State
        {
            get
            {
                return _state;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (EntityTracker.Cache.TryGet(other.gameObject, out var tracker))
            {
                OnEntityEnter(tracker);
            }
        }

        private void OnEntityEnter(EntityTracker tracker)
        {
            foreach (var component in _zoneComponents)
            {
                component.OnEntityEnter(tracker);
            }

            if (tracker.Entity.EntityType == _entityMask)
            {
                if (!PrimaryContains(tracker))
                {
                    OnPrimaryZoneEntered(tracker);
                }

                foreach (var adjacent in _adjacentZones)
                {
                    adjacent.OnSecondaryZoneEntered(tracker);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (EntityTracker.Cache.TryGet(other.gameObject, out var tracker))
            {
                OnEntityExit(tracker);
            }
        }

        private void OnEntityExit(EntityTracker tracker)
        {
            foreach (var component in _zoneComponents)
            {
                component.OnEntityExit(tracker);
            }

            if (PrimaryContains(tracker))
            {
                OnPrimaryZoneExited(tracker);
            }

            foreach (var adjacent in _adjacentZones)
            {
                if (adjacent.SecondaryContains(tracker))
                {
                    adjacent.OnSecondaryZoneExited(tracker);
                }
            }
        }

        public bool PrimaryContains(EntityTracker tracker)
        {
            return _primaryEntities.Contains(tracker);
        }

        public bool SecondaryContains(EntityTracker tracker)
        {
            return _secondaryEntities.ContainsKey(tracker);
        }

        private void OnUpdateZoneState()
        {
            ZoneState newState;

            if (_primaryEntities.Count > 0)
                newState = ZoneState.PRIMARY;
            else if (_secondaryEntities.Count > 0)
                newState = ZoneState.SECONDARY;
            else
                newState = ZoneState.NONE;

            if (newState != _state)
            {
                OnZoneStateChanged(newState, _state);
            }

            _state = newState;
        }

        private void OnZoneStateChanged(ZoneState current, ZoneState previous) 
        {
            foreach (var component in _zoneComponents)
            {
                var flags = component.TriggerFlags;
                bool currentHasFlag = (flags & current) != 0;
                bool previousHasFlag = (flags & previous) != 0;

                if (currentHasFlag && !previousHasFlag)
                {
                    component.OnZoneEnabled();
                }
                else if (!currentHasFlag && previousHasFlag)
                {
                    component.OnZoneDisabled();
                }
            }
        }

        private void OnPrimaryZoneEntered(EntityTracker tracker)
        {
            _primaryEntities.Add(tracker);

            OnUpdateZoneState();

            foreach (var component in _zoneComponents)
            {
                component.OnPrimaryZoneEntered(tracker);
            }
        }

        private void OnSecondaryZoneEntered(EntityTracker tracker)
        {
            if (!_secondaryEntities.ContainsKey(tracker))
                _secondaryEntities[tracker] = 0;

            _secondaryEntities[tracker]++;

            OnUpdateZoneState();

            foreach (var component in _zoneComponents)
            {
                component.OnSecondaryZoneEntered(tracker);
            }
        }
        private void OnPrimaryZoneExited(EntityTracker tracker)
        {
            _primaryEntities.Remove(tracker);

            OnUpdateZoneState();

            foreach (var component in _zoneComponents)
            {
                component.OnPrimaryZoneExited(tracker);
            }
        }

        private void OnSecondaryZoneExited(EntityTracker tracker)
        {
            _secondaryEntities[tracker]--;

            if (_secondaryEntities[tracker] <= 0)
                _secondaryEntities.Remove(tracker);

            OnUpdateZoneState();

            foreach (var component in _zoneComponents)
            {
                component.OnSecondaryZoneExited(tracker);
            }
        }
    }
}
