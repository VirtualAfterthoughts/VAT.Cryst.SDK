using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Proportions;

namespace VAT.Avatars.Vitals
{
    public interface ISkeletonVitals {

    }

    public interface ISkeletonVitalsT<TGroup, TPayload> : ISkeletonVitals
        where TGroup : IBoneGroupVitals
        where TPayload : IVitalsPayload {

        public void InjectDependencies(TGroup[] groups, TPayload payload);

        public void CalculateVitals();

        public void ApplyVitals();

        public float GetTotalMass();
    }
}
