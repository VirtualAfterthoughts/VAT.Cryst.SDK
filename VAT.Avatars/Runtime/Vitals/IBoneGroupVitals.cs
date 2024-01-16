using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Vitals
{
    public interface IBoneGroupVitals {
        public abstract int BoneCount { get; }

        public void CalculateVitals();

        public void ApplyVitals();

        public float GetBoneMass(int index);
    }

    public interface IBoneGroupVitalsT<TPayload> : IBoneGroupVitals
        where TPayload : IVitalsPayload {

        public void InjectDependencies(TPayload payload);
    }
}
