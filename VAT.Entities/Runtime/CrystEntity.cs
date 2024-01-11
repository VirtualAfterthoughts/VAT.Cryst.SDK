using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Entities
{
    [SelectionBase]
    public class CrystEntity : MonoBehaviour, ICrystEntity
    {
        [SerializeField]
        [Tooltip("The type of entity this is. Defaults to MISC.")]
        private CrystEntityType _entityType = CrystEntityType.MISC;

        [SerializeField]
        [Tooltip("Disable this if you want to manually add CrystEntityTrackers to colliders. Defaults to true.")]
        private bool _autoSetupTrackers = true;

        public CrystEntityType EntityType => _entityType;

        private bool _isUnloaded = false;

        public bool IsUnloaded => _isUnloaded;

        private readonly CrystEntityHierarchy _hierarchy = new();
        public CrystEntityHierarchy Hierarchy => _hierarchy;

        public event Action OnLoaded, OnUnloaded;

        private void Awake()
        {
            if (_autoSetupTrackers)
            {
                foreach (var collider in GetComponentsInChildren<Collider>())
                {
                    if (!collider.TryGetComponent<CrystEntityTracker>(out _))
                    {
                        collider.gameObject.AddComponent<CrystEntityTracker>();
                    }
                }
            }
        }

        public GameObject GetEntityGameObject()
        {
            return gameObject;
        }

        [ContextMenu("Load")]
        public void Load()
        {
            if (!IsUnloaded)
                return;

            gameObject.SetActive(true);
            _isUnloaded = false;

            OnLoaded?.Invoke();
        }

        [ContextMenu("Unload")]
        public void Unload()
        {
            if (IsUnloaded)
                return;

            OnUnloaded?.Invoke();

            _isUnloaded = true;
            gameObject.SetActive(false);
        }

        [ContextMenu("Freeze")]
        public void Freeze()
        {
            foreach (var body in _hierarchy.Bodies)
            {
                body.Freeze();
            }
        }

        [ContextMenu("Unfreeze")]
        public void Unfreeze()
        {
            foreach (var body in _hierarchy.Bodies)
            {
                body.Unfreeze();
            }
        }
    }
}
