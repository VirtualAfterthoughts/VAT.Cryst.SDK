using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Entities
{
    public class EntityIdentifier : MonoBehaviour
    {
        [SerializeField]
        private EntityType _entityType = EntityType.NPC;

        public EntityType EntityType => _entityType;
    }
}
