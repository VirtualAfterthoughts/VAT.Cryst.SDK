using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
        public float upperArmOffsetZ;
        public quaternion upperArmRotation;

        public Ellipsoid elbowEllipsoid;

        public float3 wristOffset;
        public HandProportions handProportions;
    }
}
