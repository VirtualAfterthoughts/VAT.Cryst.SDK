using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Proportions;
using VAT.Avatars.Vitals;

using VAT.Shared.Data;

namespace VAT.Avatars.Vitals
{
    public abstract class HumanoidBoneGroupVitals : IBoneGroupVitalsT<HumanoidVitalsPayload> {
        protected float[] _boneMasses;
        public virtual float[] BoneMasses { get { return _boneMasses; } }

        public abstract int BoneCount { get; }

        public float GetBoneMass(int index) {
            return BoneMasses[index];
        }

        public abstract void ConfigureJoints();
        public abstract void ApplyVitals();
        public abstract void CalculateVitals();

        public void InjectDependencies(HumanoidVitalsPayload payload) {
            _boneMasses = new float[BoneCount];

            OnInjectDependencies(payload);
        }

        protected abstract void OnInjectDependencies(HumanoidVitalsPayload payload);

        public float GetTotalMass() {
            float mass = 0f;

            for (var i = 0; i < BoneCount; i++) {
                mass += BoneMasses[i];
            }

            return mass;
        }

        protected float Internal_CalculateMass(Ellipsoid proportions, float density) {
            var volume = proportions.GetVolume();
            return volume * density;
        }
    }
}
