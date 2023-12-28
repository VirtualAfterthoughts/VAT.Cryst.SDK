using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Cryst;
using VAT.Entities;

namespace VAT.Zones
{
    public sealed class ZoneDespawner : ZoneComponent
    {
        public enum DespawnType
        {
            DESPAWN = 1 << 0,
            RESPAWN = 1 << 1,
        }

        [SerializeField]
        private DespawnType _despawnType = DespawnType.DESPAWN;

        public override void OnPrimaryZoneEntered(EntityTracker tracker)
        {
            OnDespawnEntity(tracker, ZoneState.PRIMARY);
        }

        public override void OnSecondaryZoneEntered(EntityTracker tracker)
        {
            OnDespawnEntity(tracker, ZoneState.SECONDARY);
        }

        private void OnDespawnEntity(EntityTracker tracker, ZoneState state)
        {
            var entity = tracker.Entity.GetEntityGameObject();

            if ((TriggerFlags & state) != 0)
            {
                switch (_despawnType)
                {
                    default:
                    case DespawnType.DESPAWN:
                        if (IDespawnable.Cache.TryGet(entity, out var despawnable))
                        {
                            despawnable.Despawn();
                        }
                        break;
                    case DespawnType.RESPAWN:
                        if (IRespawnable.Cache.TryGet(entity, out var respawnable))
                        {
                            respawnable.Respawn();
                        }
                        break;
                }
            }
        }
    }
}
