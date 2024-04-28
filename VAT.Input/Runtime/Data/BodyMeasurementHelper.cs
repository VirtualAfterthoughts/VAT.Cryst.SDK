using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input.Data
{
    public static class BodyMeasurementHelper
    {
        public static readonly BodyMeasurements AverageHuman = Create(1.76f);

        public const float HeadHeightPercent = 0.15f;

        public static BodyMeasurements Create(float height)
        {
            return new BodyMeasurements()
            {
                height = height,
                wingspan = height,
                chestCircumference = height * 0.59f,
            };
        }

        public static BodyMeasurements Scale(BodyMeasurements measurements, float scale)
        {
            return new BodyMeasurements()
            {
                height = measurements.height * scale,
                wingspan = measurements.wingspan * scale,
                chestCircumference = measurements.chestCircumference * scale,
            };
        }
    }
}
