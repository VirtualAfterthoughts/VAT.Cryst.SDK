using System;
using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;
using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Proportions
{
    [Serializable]
    public struct FingerProportions : IBoneGroupProportions
    {
        public SimpleTransform metaCarpalTransform;

        public SimpleTransform proximalTransform;

        public SimpleTransform middleTransform;

        public Ellipsoid proximalEllipsoid;

        public Ellipsoid middleEllipsoid;

        public Ellipsoid distalEllipsoid;

        public int phalanxCount;

        public float GetLength() {
            return proximalEllipsoid.height + middleEllipsoid.height + distalEllipsoid.height;
        }
    }
}
