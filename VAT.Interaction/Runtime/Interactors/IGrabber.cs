using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IGrabber
    {
        GameObject GetGrabberGameObject();

        Rigidbody GetGrabberRigidbody();

        SimpleTransform GetGrabPoint();

        bool CanAttach();
    }
}
