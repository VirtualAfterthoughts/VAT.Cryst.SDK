using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Proportions
{
    [Serializable]
    public struct HumanoidNeckProportions : IBoneGroupProportions {
        public Ellipse topEllipse;

        public Ellipse foreheadEllipse;

        public Ellipsoid skullEllipsoid;
        public float skullYOffset;

        public Ellipse jawEllipse;

        public Ellipsoid upperNeckEllipsoid;
        public float upperNeckOffsetZ;

        public Ellipsoid lowerNeckEllipsoid;
        public float lowerNeckOffsetZ;
    }
}
