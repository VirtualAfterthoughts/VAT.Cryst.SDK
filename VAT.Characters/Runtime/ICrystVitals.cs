using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Data;

namespace VAT.Characters
{
    public delegate void CrystVitalsDelegate(ICrystVitals vitals);

    public interface ICrystVitals
    {
        event CrystVitalsDelegate OnUpdatedVitals;

        BodyMeasurements PlayerMeasurements { get; set; }
        
        BodyMeasurements CharacterMeasurements { get; set; }

        void UpdateVitals();
    }
}