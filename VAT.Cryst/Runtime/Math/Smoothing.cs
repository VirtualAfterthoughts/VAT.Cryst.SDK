using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Cryst.Math
{
    public static class Smoothing
    {
        public static float CalculateSmoothing(float smoothing, float deltaTime)
        {
            return 1f - Mathf.Pow(smoothing, deltaTime);
        }

        public static float CalculateDecay(float decay, float deltaTime)
        {
            return 1f - Mathf.Exp(-decay * deltaTime);
        }
    }
}
