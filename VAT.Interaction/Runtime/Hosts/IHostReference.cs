using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Interaction
{
    public interface IHostReference
    {
        bool HasRigidbody { get; }

        GameObject GetHostGameObject();

        Rigidbody GetHostRigidbody();
    }
}
