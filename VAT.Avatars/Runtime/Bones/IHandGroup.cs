using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;
using VAT.Input.Data;

namespace VAT.Avatars.Bones
{
    public interface IHandGroup : IBoneGroup
    {
        IBone Hand { get; }

        IBone Palm { get; }

        IFingerGroup[] Fingers { get; }

        IThumbGroup[] Thumbs { get; }

        SimpleTransform GetPointOnPalm(Vector2 position);

        void SetOpenPose(HandPoseData data);

        void SetClosedPose(HandPoseData data);

        void SetBlendPose(HandPoseData data);

        HandPoseData GetBlendPose();
    }
}
