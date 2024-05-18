using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;
using VAT.Input.Data;
using VAT.Shared.Data;

namespace VAT.Interaction
{
    public abstract class PalmPoint
    {
        public float GetThumbDot()
        {
            return Vector3.Dot(GetUpperPalmInHost().right, -GetNormalInHost());
        }

        public Handedness GetHandedness()
        {
            return GetThumbDot() < 0f ? Handedness.LEFT : Handedness.RIGHT;
        }

        public virtual SimpleTransform GetProximityCenterInHost()
        {
            return GetUpperPalmInHost();
        }

        public virtual SimpleTransform GetUpperPalmInHost()
        {
            return GetPalmInHost(Vector2.up);
        }

        public abstract SimpleTransform GetPalmInHost(Vector2 position);

        public abstract SimpleTransform GetPressureCenterInHost(HandPoseData pose);

        public abstract Vector3 GetNormalInHost();

        public abstract SimpleTransform GetHostTransform();
    }
}
