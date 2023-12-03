using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Interaction
{
    public interface IHostManager
    {
        IReadOnlyList<IHost> Hosts { get; }

        GameObject GetManagerGameObject();
    }
}
