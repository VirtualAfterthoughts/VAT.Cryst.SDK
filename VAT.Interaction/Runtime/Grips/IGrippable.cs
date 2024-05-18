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

        SimpleTransform CalculateTargetInHost(PalmPoint point, HandPoseData pose);

        SimpleTransform CalculateDefaultTargetInHost(PalmPoint point, HandPoseData pose);

        SimpleTransform GetTargetInHost(IInteractor interactor);

        void SetTargetInHost(IInteractor interactor, SimpleTransform target);

        void ClearTargetInHost(IInteractor interactor);
    }
}
