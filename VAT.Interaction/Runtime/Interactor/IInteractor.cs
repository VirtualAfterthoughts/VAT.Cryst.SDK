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

        SimpleTransform GetGrabPoint();

        SimpleTransform GetGrabPoint(Vector2 position);

        SimpleTransform GetGrabCenter();

        Rigidbody GetRigidbody();

        void RegisterOverride(IInteractorOverride interactorOverride);

        void UnregisterOverride(IInteractorOverride interactorOverride);

        void AttachGrip(Grip grip);

        void DetachGrip(Grip grip);

        void DetachGrips();

        float GetGripForce();
    }
}
