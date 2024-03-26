using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Characters
{
    public interface ICrystRigManager
    {
        ICrystVitals GetVitalsOrDefault()
        {
            return null;
        }
    }
}
