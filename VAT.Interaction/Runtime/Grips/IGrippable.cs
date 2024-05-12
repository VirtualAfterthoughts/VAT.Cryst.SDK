using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Data;
using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IGrippable : IInteractable
    {
        event InteractorDelegate OnAttached, OnDetached;

        void OnAttachConfirm(IInteractor interactor);

        void OnAttachComplete(IInteractor interactor);

        void OnDetachConfirm(IInteractor interactor);

        void OnAttachUpdate(IInteractor interactor);

        (bool valid, HandPoseData data) GetOpenPose(IInteractor interactor);

        (bool valid, HandPoseData data) GetClosedPose(IInteractor interactor);

        HandPoseData GetDefaultPose();

        SimpleTransform GetTargetInWorld(PalmPoint point, HandPoseData pose);

        SimpleTransform GetDefaultTargetInWorld(PalmPoint point, HandPoseData pose);
    }
}
