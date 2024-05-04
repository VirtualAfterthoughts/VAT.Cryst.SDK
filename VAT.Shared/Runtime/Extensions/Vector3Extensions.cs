using UnityEngine;

using static Unity.Mathematics.math;

namespace VAT.Shared.Extensions {
    using VAT.Shared.Data;

    /// <summary>
    /// Extension methods for Vector3s.
    /// </summary>
    public static partial class Vector3Extensions {
        /// <summary>
        /// Vector3.Distance except slightly faster by immediately subtracting the vectors and comparing the magnitude.
        /// </summary>
        /// <param name="lft">The left vector.</param>
        /// <param name="rht">The right vector.</param>
        /// <returns>The distance.</returns>
        public static float FastDistance(this Vector3 lft, Vector3 rht) => (rht - lft).magnitude;

        /// <summary>
        /// Returns the maximum component of this vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The maximum component.</returns>
        public static float Maximum(this Vector3 vector) => cmax(vector);

        /// <summary>
        /// Flattens the forward vector as if its a neck, using an up vector.
        /// </summary>
        /// <param name="forward">The forward vector.</param>
        /// <param name="up">The up vector.</param>
        /// <param name="root">The relative root vector (ex. the character controller's up). Defaults to world space up.</param>
        /// <returns>The flattened vector.</returns>
        public static Vector3 FlattenNeck(this Vector3 forward, Vector3 up, Vector3? root = null) {
            if (!root.HasValue)
                root = Vector3.up;

            return Quaternion.AngleAxis(-90f, root.Value) * Vector3.Cross(root.Value, Quaternion.FromToRotation(up, root.Value) * forward).normalized;
        }

        /// <summary>
        /// Compares two Vector3s and returns true if they are similar.
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rht"></param>
        /// <returns></returns>
        public static bool Approximately(this Vector3 lft, Vector3 rht) {
            return Mathf.Approximately(lft.x, rht.x) && Mathf.Approximately(lft.y, rht.y) && Mathf.Approximately(lft.z, rht.z);
        }

        /// <summary>
        /// Generate a JSON representation of this Vector3.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static string ToJson(this Vector3 vector) => JsonUtility.ToJson(vector, false);
    }
}
