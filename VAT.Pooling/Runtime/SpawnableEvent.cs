using System;

using UnityEngine;
using UnityEngine.Events;

namespace VAT.Pooling
{
    [Serializable]
    public class SpawnableEvent : UnityEvent<GameObject, Spawner>
    {
    }
}
