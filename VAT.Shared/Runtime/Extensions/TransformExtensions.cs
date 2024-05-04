using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System;

using Object = UnityEngine.Object;

using static Unity.Mathematics.math;

#if USE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace VAT.Shared.Extensions {
    using Unity.Burst;
    using Unity.Mathematics;

    /// <summary>
    /// Extension methods for Transforms.
    /// </summary>
    public static partial class TransformExtensions {
        /// <summary>
        /// Ensures the parent is set even when called in a method that does not allow parent setting.
        /// Requires UniTask to function properly.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parent"></param>
        public static void EnsureParent(this Transform transform, Transform parent, Action onFinish = null) {
            if (transform == null)
                return;

            transform.parent = parent;

#if USE_UNITASK
            EnsureParentAsync(transform, parent, onFinish).Forget();
#endif
        }

#if USE_UNITASK
        private static async UniTaskVoid EnsureParentAsync(Transform transform, Transform parent, Action onFinish = null) {
            while (transform.parent != parent) {
                transform.parent = parent;
                await UniTask.Yield();
            }

            if (onFinish != null)
                onFinish?.Invoke();
        }
#endif

        /// <summary>
        /// Transforms rotation from local space to world space.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Quaternion TransformRotation(this Transform transform, Quaternion rotation) => transform.rotation * rotation;

        /// <summary>
        /// Transforms rotation from world space to local space.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Quaternion InverseTransformRotation(this Transform transform, Quaternion rotation) => Quaternion.Inverse(transform.rotation) * rotation;

        /// <summary>
        /// Resets the local position, rotation, and scale of this transform to default values.
        /// </summary>
        /// <param name="transform"></param>
        public static void Reset(this Transform transform) {
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localScale = Vector3.one;
        }
    }

    [BurstCompile(FloatMode = FloatMode.Fast)]
    public static class BurstCompiled_Transform
    {
        [BurstCompile(FloatMode = FloatMode.Fast)]
        public static void BurstCompiled_TransformPoint(in float3 input, in float3 position, in quaternion rotation, in float3 lossyScale, out float3 result) => result = mul(rotation, input * lossyScale) + position;

        [BurstCompile(FloatMode = FloatMode.Fast)]
        public static void BurstCompiled_TransformDirection(in float3 input, in quaternion rotation, out float3 result) => result = mul(rotation, input);

        [BurstCompile(FloatMode = FloatMode.Fast)]
        public static void BurstCompiled_TransformVector(in float3 input, in quaternion rotation, in float3 lossyScale, out float3 result) => BurstCompiled_TransformDirection(input * lossyScale, rotation, out result);

        [BurstCompile(FloatMode = FloatMode.Fast)]
        public static void BurstCompiled_TransformRotation(in quaternion input, in quaternion rotation, in float3 lossyScale, out quaternion result) {
            result = mul(rotation, input); 
        }

        [BurstCompile(FloatMode = FloatMode.Fast)]
        public static void BurstCompiled_InverseTransformPoint(in float3 input, in float3 position, in quaternion rotation, in float3 lossyScale, out float3 result) => result = mul(inverse(rotation), input - position) / lossyScale;

        [BurstCompile(FloatMode = FloatMode.Fast)]
        public static void BurstCompiled_InverseTransformDirection(in float3 input, in quaternion rotation, out float3 result) => result = mul(inverse(rotation), input);

        [BurstCompile(FloatMode = FloatMode.Fast)]
        public static void BurstCompiled_InverseTransformVector(in float3 input, in quaternion rotation, in float3 lossyScale, out float3 result)
        {
            BurstCompiled_InverseTransformDirection(input, rotation, out float3 vector);
            result = vector / lossyScale;
        }

        [BurstCompile(FloatMode = FloatMode.Fast)]
        public static void BurstCompiled_InverseTransformRotation(in quaternion input, in quaternion rotation, in float3 lossyScale, out quaternion result) {
            result = mul(inverse(rotation), input);
        }
    }
}

