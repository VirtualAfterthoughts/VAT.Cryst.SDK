using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public interface IHost : IHostReference
    {
        IReadOnlyList<IHostable> Hostables { get; }

        GameObject GetManagerGameObject();
    }
}
