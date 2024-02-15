using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public interface IHost
    {
        void EnableInteraction();

        void DisableInteraction();

        GameObject GetGameObject();

        Collider[] GetColliders();

        Rigidbody GetRigidbodyOrDefault();
    }
}
