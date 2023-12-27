using System;
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

        private bool _isUnloaded = false;

        public bool IsUnloaded => _isUnloaded;

        public event Action OnLoaded, OnUnloaded;

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

            gameObject.SetActive(false);
            _isUnloaded = true;
        }
    }
}
