using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IGrippable : IInteractable
    {
        void OnAttachConfirm(IInteractor interactor);

        void OnAttachComplete(IInteractor interactor);

        void OnDetachConfirm(IInteractor interactor);

        void OnAttachUpdate(IInteractor interactor);

        (bool valid, HandPoseData data) GetOpenPose(IInteractor interactor);

        (bool valid, HandPoseData data) GetClosedPose(IInteractor interactor);

        SimpleTransform GetTargetInWorld(IGrabberPoint grabberPoint);

        SimpleTransform GetTargetInInteractor(IGrabberPoint grabberPoint);
    }
}
