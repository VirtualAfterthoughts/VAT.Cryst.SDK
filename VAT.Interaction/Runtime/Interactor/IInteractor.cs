using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Skeleton;

namespace VAT.Interaction
{
    public delegate void InteractorDelegate(IInteractor interactor);

    public interface IInteractor
    {
        IInteractable GetHoveringInteractable() 
        { 
            return null; 
        }

        IInteractable GetFarHoveringInteractable()
        {
            return null;
        }

        InteractorState GetInteractorState();

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

        IHand GetHandOrNull();
    }
}
