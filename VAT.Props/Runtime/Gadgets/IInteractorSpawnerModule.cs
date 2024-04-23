using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Interaction;
using VAT.Packaging;

namespace VAT.Props
{
    public interface IInteractorSpawnerModule : IInteractorModule
    {
        void SetSpawningActive(bool active);

        event Action<SpawnableContentReference> OnSpawnableSelected;
    }
}
