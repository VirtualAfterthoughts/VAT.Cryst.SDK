using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace VAT.Pooling
{
    [Serializable]
    public class SpawnableEvent : UnityEvent<GameObject, SpawnablePlacer>
    {
    }
}
