using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;
using VAT.Input.Data;

namespace VAT.Avatars.Bones
{
    public interface IHandGroup : IBoneGroup
    {
        public IBone Hand { get; }

        public IBone Palm { get; }

        public IFingerGroup[] Fingers { get; }

        public IThumbGroup[] Thumbs { get; }

        SimpleTransform GetPointOnPalm(Vector2 position);

        void SetOpenPose(HandPoseData data);

        void SetClosedPose(HandPoseData data);

        void SetBlendPose(HandPoseData data);
    }
}
