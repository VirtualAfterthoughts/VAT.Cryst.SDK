using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IPalmPoint
    {
        public float GetThumbDot()
        {
            return Vector3.Dot(GetDefaultPoint().right, -GetNormal());
        }

        public Handedness GetHandedness()
        {
            return GetThumbDot() < 0f ? Handedness.LEFT : Handedness.RIGHT;
        }

        SimpleTransform GetDefaultPoint();

        SimpleTransform GetPoint(Vector2 position);

        Vector3 GetNormal();

        SimpleTransform GetProximityCenter();

        SimpleTransform GetHostTransform();
    }
}
