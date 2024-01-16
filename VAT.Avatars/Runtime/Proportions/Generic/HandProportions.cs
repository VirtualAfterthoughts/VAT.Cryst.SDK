using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Proportions
{
    [Serializable]
    public struct HandProportions : IBoneGroupProportions
    {
        public int FingerCount => fingerProportions.Length;

        public Ellipsoid wristEllipsoid;

        public Ellipsoid knuckleEllipsoid;

        public FingerProportions[] fingerProportions;
    }
}
