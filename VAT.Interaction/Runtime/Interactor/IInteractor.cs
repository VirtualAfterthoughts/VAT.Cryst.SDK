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

        InteractorTargetData GetTargetData();

        IGrabPoint GetGrabberPoint();

        Rigidbody GetRigidbody();

        void RegisterOverride(IInteractorOverride interactorOverride);

        void UnregisterOverride(IInteractorOverride interactorOverride);

        void AttachGrip(IGrippable grip);

        void DetachGrip(IGrippable grip);

        void DetachGrips();

        float GetGripForce();
    }
}
