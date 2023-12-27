using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Utilities;

namespace VAT.Cryst
{
    public interface IDespawnable
    {
        public static ComponentCache<IDespawnable> Cache = new();

        void Despawn();
    }

    public interface IRespawnable
    {
        public static ComponentCache<IRespawnable> Cache = new();

        void Respawn();
    }
}
