using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.REWORK
{
    public interface IHandGroup : IBoneGroup
    {
        public IBone Hand { get; }

        public IBone Palm { get; }

        public IFingerGroup[] Fingers { get; }

        SimpleTransform GetPointOnPalm(Vector2 position);
    }
}
