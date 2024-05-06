using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Entities
{
    public interface ICrystEntity : IFreezable
    {
        CrystEntityType EntityType { get; }

        event Action OnLoaded, OnUnloaded;

        bool IsUnloaded { get; }

        CrystEntityHierarchy Hierarchy { get; }

        GameObject GetEntityGameObject();

        void Load();

        void Unload();
    }
}
