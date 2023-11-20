using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;
using VAT.Shared.Extensions;

namespace VAT.Shared.Utilities
{
    public sealed class Float3DerivativeTracker : DerivativeTrackerT<float3>
    {
        public Float3DerivativeTracker(int count) : base(count) { }

        protected override float3 CalculateDerivative(float3 from, float3 to, float deltaTime)
        {
            return PhysicsExtensions.GetLinearVelocity(from, to, deltaTime);
        }
    }
}
