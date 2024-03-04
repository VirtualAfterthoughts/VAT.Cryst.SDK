using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars;
using VAT.Input;
using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IGrippable : IInteractable, IGrabTarget
    {
        void OnAttachConfirm(IInteractor interactor);

        void OnAttachComplete(IInteractor interactor);

        void OnDetachConfirm(IInteractor interactor);

        void OnAttachUpdate(IInteractor interactor);

        (bool valid, HandPoseData data) GetOpenPose(IInteractor interactor);

        (bool valid, HandPoseData data) GetClosedPose(IInteractor interactor);
    }
}
