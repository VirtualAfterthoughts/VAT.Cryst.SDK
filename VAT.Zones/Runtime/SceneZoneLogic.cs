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
        private readonly List<EntityIdentifier> _primaryEntities = new();
        private readonly Dictionary<EntityIdentifier, int> _secondaryEntities = new();

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
            if (EntityIdentifier.Cache.TryGet(other.gameObject, out var identifier))
            {
                OnEntityEnter(identifier);
            }
        }

        private void OnEntityEnter(EntityIdentifier identifier)
        {
            if (identifier.EntityType == _entityMask)
            {
                if (!PrimaryContains(identifier))
                {
                    OnPrimaryZoneEntered(identifier);
                }

                foreach (var adjacent in _adjacentZones)
                {
                    adjacent.OnSecondaryZoneEntered(identifier);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (EntityIdentifier.Cache.TryGet(other.gameObject, out var identifier))
            {
                OnEntityExit(identifier);
            }
        }

        private void OnEntityExit(EntityIdentifier identifier)
        {
            if (PrimaryContains(identifier))
            {
                OnPrimaryZoneExited(identifier);
            }

            foreach (var adjacent in _adjacentZones)
            {
                if (adjacent.SecondaryContains(identifier))
                {
                    adjacent.OnSecondaryZoneExited(identifier);
                }
            }
        }

        public bool PrimaryContains(EntityIdentifier identifier)
        {
            return _primaryEntities.Contains(identifier);
        }

        public bool SecondaryContains(EntityIdentifier identifier)
        {
            return _secondaryEntities.ContainsKey(identifier);
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

        private void OnPrimaryZoneEntered(EntityIdentifier identifier)
        {
            _primaryEntities.Add(identifier);

            OnUpdateZoneState();

            foreach (var component in _zoneComponents)
            {
                component.OnPrimaryZoneEntered(identifier);
            }
        }

        private void OnSecondaryZoneEntered(EntityIdentifier identifier)
        {
            if (!_secondaryEntities.ContainsKey(identifier))
                _secondaryEntities[identifier] = 0;

            _secondaryEntities[identifier]++;

            OnUpdateZoneState();

            foreach (var component in _zoneComponents)
            {
                component.OnSecondaryZoneEntered(identifier);
            }
        }
        private void OnPrimaryZoneExited(EntityIdentifier identifier)
        {
            _primaryEntities.Remove(identifier);

            OnUpdateZoneState();

            foreach (var component in _zoneComponents)
            {
                component.OnPrimaryZoneExited(identifier);
            }
        }

        private void OnSecondaryZoneExited(EntityIdentifier identifier)
        {
            _secondaryEntities[identifier]--;

            if (_secondaryEntities[identifier] <= 0)
                _secondaryEntities.Remove(identifier);

            OnUpdateZoneState();

            foreach (var component in _zoneComponents)
            {
                component.OnSecondaryZoneExited(identifier);
            }
        }
    }
}
