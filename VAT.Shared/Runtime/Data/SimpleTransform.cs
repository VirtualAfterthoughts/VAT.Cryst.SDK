using System;

using static Unity.Mathematics.math;

using UnityEngine;

using VAT.Shared.Extensions;

namespace VAT.Shared.Data {
    using Unity.Mathematics;

    /// <summary>
    /// A data structure containing a snapshot of a transform.
    /// </summary>
    [Serializable]
    public struct SimpleTransform {
        /// <summary>
        /// A SimpleTransform with default values.
        /// </summary>
        public static readonly SimpleTransform Default = Create(float3.zero, quaternion.identity);

        /// <summary>
        /// The world space position of the snapshot.
        /// </summary>
        public float3 position;

        /// <summary>
        /// The world space rotation of the snapshot.
        /// </summary>
        public quaternion rotation;

        /// <summary>
        /// The global scale of the snapshot.
        /// </summary>
        public float3 lossyScale;

        /// <summary>
        /// Matrix that transforms a point from local space into world space.
        /// </summary>
        public Matrix4x4 localToWorldMatrix;

        /// <summary>
        /// The forward vector of the snapshot.
        /// </summary>
        public float3 forward => math.mul(rotation, math.forward());

        /// <summary>
        /// The up vector of the snapshot.
        /// </summary>
        public float3 up => math.mul(rotation, math.up());

        /// <summary>
        /// The right vector of the snapshot.
        /// </summary>
        public float3 right => math.mul(rotation, math.right());

        /// <summary>
        /// Creates a snapshot of this transform.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static SimpleTransform Create(Transform transform) {
            return Create(transform.position, transform.rotation, transform.lossyScale);
        }

        /// <summary>
        /// Creates a snapshot of this position and rotation with a scale of (1, 1, 1).
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static SimpleTransform Create(float3 position, quaternion rotation)
        {
            return Create(position, rotation, 1f);
        }

        /// <summary>
        /// Creates a snapshot of this position, rotation, and scale.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="lossyScale"></param>
        /// <returns></returns>
        public static SimpleTransform Create(float3 position, quaternion rotation, float3 lossyScale)
        {
            SimpleTransform simple;
            simple.position = position;
            simple.rotation = normalize(rotation);
            simple.lossyScale = lossyScale;
            simple.localToWorldMatrix = Matrix4x4.TRS(simple.position, simple.rotation, simple.lossyScale);
            return simple;
        }

        /// <summary>
        /// Transforms a SimpleTransform from local space to world space.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public SimpleTransform Transform(SimpleTransform transform)
        {
            return Create(TransformPoint(transform.position), TransformRotation(transform.rotation), transform.lossyScale * lossyScale);
        }

        /// <summary>
        /// Transforms position from local space to world space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float3 TransformPoint(float3 position) {
            BurstCompiled_Transform.BurstCompiled_TransformPoint(position, this.position, rotation, lossyScale, out var result);
            return result;
        }

        /// <summary>
        /// Transforms direction from local space to world space.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public float3 TransformDirection(float3 direction) {
            BurstCompiled_Transform.BurstCompiled_TransformDirection(direction, rotation, out var result);
            return result;
        }

        /// <summary>
        /// Transforms vector from local space to world space.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float3 TransformVector(float3 vector) {
            BurstCompiled_Transform.BurstCompiled_TransformVector(vector, rotation, lossyScale, out var result);
            return result;
        }

        /// <summary>
        /// Transforms rotation from local space to world space.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public quaternion TransformRotation(quaternion rotation) {
            BurstCompiled_Transform.BurstCompiled_TransformRotation(rotation, this.rotation, this.lossyScale, out var result);
            return result;
        }

        /// <summary>
        /// Transforms a SimpleTransform from world space to local space.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public SimpleTransform InverseTransform(SimpleTransform transform) {
            return Create(InverseTransformPoint(transform.position), InverseTransformRotation(transform.rotation), transform.lossyScale / lossyScale);
        }

        /// <summary>
        /// Transforms position from world space to local space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float3 InverseTransformPoint(float3 position) {
            BurstCompiled_Transform.BurstCompiled_InverseTransformPoint(position, this.position, rotation, lossyScale, out var result);
            return result;
        }

        /// <summary>
        /// Transforms direction from world space to local space.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public float3 InverseTransformDirection(float3 direction) {
            BurstCompiled_Transform.BurstCompiled_InverseTransformDirection(direction, rotation, out var result);
            return result;
        }

        /// <summary>
        /// Transforms vector from world space to local space.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float3 InverseTransformVector(float3 vector) {
            BurstCompiled_Transform.BurstCompiled_InverseTransformVector(vector, rotation, lossyScale, out var result);
            return result;
        }

        /// <summary>
        /// Transforms rotation from world space to local space.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public quaternion InverseTransformRotation(quaternion rotation) { 
            BurstCompiled_Transform.BurstCompiled_InverseTransformRotation(rotation, this.rotation, lossyScale, out var result);
            return result;
        }

        /// <summary>
        /// Creates a SimpleTransform that is t percent between a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static SimpleTransform Lerp(SimpleTransform a, SimpleTransform b, float t) {
            return Create(
                lerp(a.position, b.position, t),
                slerp(a.rotation, b.rotation, t),
                lerp(a.lossyScale, b.lossyScale, t)
            );
        }

        public static implicit operator SimpleTransform(Transform transform) => Create(transform);
    }
}
