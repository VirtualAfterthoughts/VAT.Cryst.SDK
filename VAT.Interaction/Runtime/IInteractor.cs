using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Entities;
using VAT.Entities.PhysX;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IInteractor
    {
        bool IsInteractionLocked();

        void LockInteraction();

        void UnlockInteraction();

        SimpleTransform GetGrabPoint();

        CrystRigidbody GetRigidbody();
    }
}
