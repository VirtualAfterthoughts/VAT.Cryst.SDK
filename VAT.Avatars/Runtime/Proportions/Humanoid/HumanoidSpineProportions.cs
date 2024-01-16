using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Proportions
{
    [Serializable]
    public struct HumanoidSpineProportions : IBoneGroupProportions
    {
        public Ellipsoid upperChestEllipsoid;
        public float upperChestOffsetZ;

        public Ellipsoid chestEllipsoid;
        public float chestOffsetZ;

        public Ellipsoid spineEllipsoid;
        public float spineOffsetZ;

        public Ellipsoid pelvisEllipsoid;
        public float pelvisOffsetZ;
    }
}
