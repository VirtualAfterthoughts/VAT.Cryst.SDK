using System;
using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Proportions
{
    [Serializable]
    public struct HumanoidLegProportions : IBoneGroupProportions {
        public Ellipsoid hipEllipsoid;
        public float hipSeparationOffset;

        public Ellipsoid kneeEllipsoid;
        public float kneeOffsetZ;

        public Ellipsoid ankleEllipsoid;
        public float ankleOffsetZ;

        public Ellipsoid toeEllipsoid;
        public float3 toeOffset;

        public float GetLength() {
            return hipEllipsoid.height + kneeEllipsoid.height;
        }
    }
}
