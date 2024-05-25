using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Data;
using VAT.Input.Skeleton;

namespace VAT.Interaction
{
    public delegate void InteractorDelegate(IInteractor interactor);

    public interface IInteractor
    {
        PhysLimb GetLimb();

        bool IsInteractionLocked();

        void LockInteraction();

        void UnlockInteraction();

        InteractorTargetData GetTargetData();

        PalmPoint GetPalm();

        Rigidbody GetRigidbody();

        void RegisterOverride(IInteractorOverride interactorOverride);

        void UnregisterOverride(IInteractorOverride interactorOverride);

        void AttachGrip(IGrippable grip);

        void DetachGrip(IGrippable grip);

        void DetachGrips();

        TModule GetModule<TModule>() where TModule : IInteractorModule;

        void RegisterModule(IInteractorModule module);

        void DeregisterModule(IInteractorModule module);

        IInputHand GetInputHand();

        void ResetPose();

        void SetClosedPose(HandPoseData pose);

        void SetOpenPose(HandPoseData pose);
    }
}
