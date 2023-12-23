using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Shared.Utilities;

namespace VAT.Entities
{
    [RequireComponent(typeof(Collider))]
    public class EntityIdentifier : MonoBehaviour
    {
        public static ComponentCache<EntityIdentifier> Cache = new();

        [SerializeField]
        private EntityType _entityType = EntityType.NPC;

        public EntityType EntityType => _entityType;

        private void OnEnable()
        {
            Cache.Add(gameObject, this);
        }

        private void OnDisable()
        {
            Cache.Remove(gameObject, this);
        }
    }
}
