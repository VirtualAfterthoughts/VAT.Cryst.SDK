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

        public float GetHeight()
        {
            float height = 0f;
            var leg = leftLegProportions;
            height += leg.GetLength() + leg.ankleEllipsoid.height;
            height += spineProportions.GetLength() + -spineProportions.GetHipYOffset();

            height += neckProportions.GetLength();

            return height;
        }

        public float GetWingspan()
        {
            float wingspan = 0f;

            var leftArm = leftArmProportions;
            var rightArm = rightArmProportions;

            wingspan += leftArm.GetLength() + leftArm.handProportions.GetLength();
            wingspan += rightArm.GetLength() + rightArm.handProportions.GetLength();

            wingspan += spineProportions.upperChestEllipsoid.radius.x * 2f;

            return wingspan;
        }

        private float GetCircumference(float a, float b)
        {
            return 2f * Mathf.PI * Mathf.Sqrt((a * a + b * b) / 2f);
        }

        public float GetChestCircumference()
        {
            var upperChest = spineProportions.upperChestEllipsoid;

            return GetCircumference(upperChest.radius.x, upperChest.radius.y);
        }

        public BodyMeasurements GetMeasurements()
        {
            float height = GetHeight();

            float wingspan = GetWingspan();

            float chestCircumference = GetChestCircumference();

            return new BodyMeasurements()
            {
                height = height,
                wingspan = wingspan,
                chestCircumference = chestCircumference,
            };
        }
    }
}
