using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public abstract class PalmPoint
    {
        public float GetThumbDot()
        {
            return Vector3.Dot(GetDefaultPoint().right, -GetNormal());
        }

        public Handedness GetHandedness()
        {
            return GetThumbDot() < 0f ? Handedness.LEFT : Handedness.RIGHT;
        }

        public virtual SimpleTransform GetDefaultPoint()
        {
            return GetPoint(Vector2.zero);
        }

        public virtual SimpleTransform GetProximityCenter()
        {
            return GetDefaultPoint();
        }

        public abstract SimpleTransform GetPoint(Vector2 position);

        public abstract Vector3 GetNormal();

        public abstract SimpleTransform GetHostTransform();
    }
}
