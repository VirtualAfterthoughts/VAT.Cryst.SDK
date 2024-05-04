using UnityEngine;

namespace VAT.Shared.Extensions {
    /// <summary>
    /// Extension methods for floats.
    /// </summary>
    public static partial class FloatExtensions {
        /// <summary>
        /// Returns the average of each float.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Average(params float[] values) {
            return Sum(values) / values.Length;
        }

        /// <summary>
        /// Returns the sum of each float.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Sum(params float[] values) {
            float t = 0f;
            for (int i = 0; i < values.Length; i++)
                t += values[i];
            return t;
        }

        /// <summary>
        /// Returns the float rounded to the nearest int.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float Rounded(this float f) => Mathf.Round(f);

        /// <summary>
        /// Clamps float f between -1 and 1.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float SinClamp(this float f) => Mathf.Clamp(f, -1f, 1f);
    }
}
