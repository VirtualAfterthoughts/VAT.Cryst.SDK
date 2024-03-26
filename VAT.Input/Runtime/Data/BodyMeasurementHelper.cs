using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input.Data
{
    public static class BodyMeasurementHelper
    {
        public static readonly BodyMeasurements AverageHuman = Create(1.76f);

        public static BodyMeasurements Create(float height)
        {
            return new BodyMeasurements()
            {
                height = height,
                wingspan = height,
            };
        }
    }
}
