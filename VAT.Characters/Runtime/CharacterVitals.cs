using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Data;

namespace VAT.Characters
{
    public class CharacterVitals : MonoBehaviour, ICrystVitals
    {
        [SerializeField]
        private BodyMeasurements _playerMeasurements = BodyMeasurementHelper.AverageHuman;

        private BodyMeasurements _characterMeasurements = BodyMeasurementHelper.AverageHuman;

        public BodyMeasurements PlayerMeasurements { get => _playerMeasurements; set => _playerMeasurements = value; }
        public BodyMeasurements CharacterMeasurements { get => _characterMeasurements; set => _characterMeasurements = value; }

        public event CrystVitalsDelegate OnUpdatedVitals;

        [ContextMenu("Update Vitals")]
        public void UpdateVitals()
        {
            OnUpdatedVitals?.Invoke(this);
        }
    }
}
