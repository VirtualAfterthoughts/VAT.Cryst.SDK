using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Entities;
using VAT.Cryst;

namespace VAT.Misc
{
    [RequireComponent(typeof(Collider))]
    public sealed class EntityDespawner : MonoBehaviour
    {
        public enum DespawnType
        {
            DESPAWN = 1 << 0,
            RESPAWN = 1 << 1,
        }

        [SerializeField]
        private EntityType _entityMask = ~EntityType.PLAYER;

        [SerializeField]
        private DespawnType _despawnType = DespawnType.DESPAWN;


        private void OnTriggerEnter(Collider other)
        {
            if (EntityTracker.Cache.TryGet(other.gameObject, out var tracker))
            {
                OnDespawnEntity(tracker);
            }
        }

        private void OnDespawnEntity(EntityTracker tracker)
        {
            var entity = tracker.Entity;

            if ((_entityMask & entity.EntityType) != 0)
            {
                var entityGameObject = entity.GetEntityGameObject();

                switch (_despawnType)
                {
                    default:
                    case DespawnType.DESPAWN:
                        if (IDespawnable.Cache.TryGet(entityGameObject, out var despawnable))
                        {
                            despawnable.Despawn();
                        }
                        break;
                    case DespawnType.RESPAWN:
                        if (IRespawnable.Cache.TryGet(entityGameObject, out var respawnable))
                        {
                            respawnable.Respawn();
                        }
                        break;
                }
            }
        }
    }
}
