using System;
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

        public event Action<EntityTracker> OnEnabled, OnDisabled;

        private void Awake()
        {
            _entity = GetComponentInParent<CrystEntity>(true);

            if (_entity == null)
            {
                Debug.LogError($"Entity Tracker {name} is not a child of a CrystEntity!", this);
                enabled = false;
                return;
            }

            Cache.Add(gameObject, this);
        }

        private void OnDestroy()
        {
            Cache.Remove(gameObject, this);
        }

        private void OnEnable()
        {
            OnEnabled?.Invoke(this);
        }

        private void OnDisable()
        {
            OnDisabled?.Invoke(this);
        }
    }
}
