using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Characters
{
    public interface ICrystRigManager
    {
        event Action<float> OnManagerUpdate, OnManagerFixedUpdate, OnManagerLateUpdate;

        ICrystVitals GetVitalsOrNull();
    }
}
