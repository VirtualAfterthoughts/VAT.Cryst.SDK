using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Shared.Utilities;

namespace VAT.Entities
{
    [RequireComponent(typeof(Collider))]
    public class CrystEntityTracker : MonoBehaviour
    {
        public static ComponentCache<CrystEntityTracker> Cache = new();

        private ICrystEntity _entity = null;

        public ICrystEntity Entity => _entity;

        public event Action<CrystEntityTracker> OnEnabled, OnDisabled;

        private void Awake()
        {
            _entity = GetComponentInParent<ICrystEntity>(true);

            if (_entity == null)
            {
                Debug.LogError($"Entity Tracker {name} is not a child of an Entity!", this);
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
