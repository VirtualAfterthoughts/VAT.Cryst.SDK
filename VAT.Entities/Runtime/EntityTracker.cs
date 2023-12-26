using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Shared.Utilities;

namespace VAT.Entities
{
    [RequireComponent(typeof(Collider))]
    public class EntityTracker : MonoBehaviour
    {
        public static ComponentCache<EntityTracker> Cache = new();

        private CrystEntity _entity = null;

        public CrystEntity Entity => _entity;

        private void Awake()
        {
            _entity = GetComponentInParent<CrystEntity>(true);

            if (_entity == null)
            {
                Debug.LogError($"Entity Tracker {name} is not a child of a CrystEntity!", this);
                enabled = false;
                return;
            }
        }

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
