using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Entities
{
    public class CrystEntity : MonoBehaviour, IEntity
    {
        [SerializeField]
        private EntityType _entityType = EntityType.MISC;

        [SerializeField]
        private bool _autoSetupTrackers = true;

        public EntityType EntityType => _entityType;

        private bool _isUnloaded = false;

        public bool IsUnloaded => _isUnloaded;

        public event Action OnLoaded, OnUnloaded;

        private void Awake()
        {
            if (_autoSetupTrackers)
            {
                foreach (var collider in GetComponentsInChildren<Collider>())
                {
                    if (!collider.TryGetComponent<EntityTracker>(out _))
                    {
                        collider.gameObject.AddComponent<EntityTracker>();
                    }
                }
            }
        }

        public GameObject GetEntityGameObject()
        {
            return gameObject;
        }

        public void Load()
        {
            if (!IsUnloaded)
                return;

            gameObject.SetActive(true);
            _isUnloaded = false;

            OnLoaded?.Invoke();
        }

        public void Unload()
        {
            if (IsUnloaded)
                return;

            OnUnloaded?.Invoke();

            _isUnloaded = true;
            gameObject.SetActive(false);
        }
    }
}
