using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Data;

namespace VAT.Avatars.Proportions {
    [Serializable]
    public struct HumanoidProportions : ISkeletonProportions {
        public HumanoidNeckProportions neckProportions;

        public HumanoidSpineProportions spineProportions;

        public HumanoidArmProportions leftArmProportions;
        public HumanoidArmProportions rightArmProportions;

        public HumanoidLegProportions leftLegProportions;
        public HumanoidLegProportions rightLegProportions;

        public BodyMeasurements GetMeasurements()
        {
            float height = 0f;
            var leg = leftLegProportions;
            height += leg.GetLength() + leg.ankleEllipsoid.height;
            height += spineProportions.GetLength() + -spineProportions.GetHipYOffset();

            height += neckProportions.GetLength();

            float wingspan = 0f;

            var leftArm = leftArmProportions;
            var rightArm = rightArmProportions;

            wingspan += leftArm.GetLength() + leftArm.handProportions.GetLength();
            wingspan += rightArm.GetLength() + rightArm.handProportions.GetLength();

            wingspan += spineProportions.upperChestEllipsoid.radius.x * 2f;

            return new BodyMeasurements()
            {
                height = height,
                wingspan = wingspan,
            };
        }
    }
}
