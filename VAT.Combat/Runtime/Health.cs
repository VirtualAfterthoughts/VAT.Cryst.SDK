using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        [Min(0f)]
        private float _maxHealth = 100f;

        private float _currentHealth = 0f;

        public float CurrentHealth
        {
            get
            {
                return _currentHealth;
            }
        }

        public float MaxHealth
        {
            get
            {
                return _maxHealth;
            }
        }

        public float HealthPercent => CurrentHealth / MaxHealth;

        public void SetFullHealth()
        {
            _currentHealth = _maxHealth;
        }
    }
}
