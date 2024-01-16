using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Proportions
{
    [Serializable]
    public struct HumanoidArmProportions : IBoneGroupProportions
    {
        public Ellipsoid clavicleEllipsoid;
        public float clavicleSeparation;

        public Ellipsoid shoulderBladeEllipsoid;

        public Ellipsoid upperArmEllipsoid;

        public Ellipsoid elbowEllipsoid;

        public HandProportions handProportions;
    }
}
