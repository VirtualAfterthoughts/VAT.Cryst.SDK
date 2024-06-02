using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Cryst.Math
{
    using System;

    public static class Smoothing
    {
        public static float CalculateInterpolation(double smoothing, double deltaTime)
        {
            return (float)(1f - Math.Pow(smoothing, deltaTime));
        }
    }
}
