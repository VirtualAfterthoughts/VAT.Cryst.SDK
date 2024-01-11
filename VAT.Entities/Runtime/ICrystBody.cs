using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace VAT.Entities
{
    public interface ICrystBody : IFreezable
    {
        float Mass { get; set; }

        float3 Position { get; set; }
        quaternion Rotation { get; set; }

        float3 Velocity { get; set; }
        float3 AngularVelocity { get; set; }

        bool HasBody { get; }
    }
}
