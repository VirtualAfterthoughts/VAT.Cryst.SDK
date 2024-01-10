using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

namespace VAT.Entities {
    public abstract class CrystJointSpace {
        public abstract float3 RawTargetPosition { get; set; }
        public abstract quaternion RawTargetRotation { get; set; }
        public abstract float3 RawTargetVelocity { get; set; }
        public abstract float3 RawTargetAngularVelocity { get; set; }

        /// <summary>
        /// Transforms the target rotation into joint space.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public abstract quaternion InverseTransformTargetRotation(quaternion target, CrystSpace space = CrystSpace.WORLD);

        /// <summary>
        /// Transforms the target position into joint space.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public abstract float3 InverseTransformTargetPosition(float3 target, CrystSpace space = CrystSpace.WORLD);
    }
}
