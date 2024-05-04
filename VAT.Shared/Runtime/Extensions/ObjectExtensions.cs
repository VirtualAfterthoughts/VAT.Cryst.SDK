using System.Collections;
using System.Collections.Generic;

namespace VAT.Shared.Extensions {
    using UnityEngine;

    public static partial class ObjectExtensions {
        /// <summary>
        /// Removes a GameObject, component, or asset if it exists.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool TryDestroy(this Object obj) {
            return TryDestroy(obj, 0f);
        }

        /// <summary>
        /// Removes a GameObject, component, or asset if it exists.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool TryDestroy(this Object obj, float t)
        {
            if (obj != null)
            {
                Object.Destroy(obj, t);
                return true;
            }

            return false;
        }
    }
}
