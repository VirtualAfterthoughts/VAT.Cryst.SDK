using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Entities
{
    public class CrystEntity : MonoBehaviour
    {
        [SerializeField]
        private EntityType _entityType = EntityType.MISC;

        public EntityType EntityType => _entityType;
    }
}
