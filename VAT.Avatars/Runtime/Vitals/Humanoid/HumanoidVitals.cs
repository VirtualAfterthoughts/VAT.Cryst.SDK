using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Vitals {
    public class HumanoidVitals : ISkeletonVitalsT<HumanoidBoneGroupVitals, HumanoidVitalsPayload> {
        private HumanoidBoneGroupVitals[] _groups;

        public void InjectDependencies(HumanoidBoneGroupVitals[] groups, HumanoidVitalsPayload payload) {
            _groups = groups;

            for (var i = 0; i < groups.Length; i++) {
                groups[i].InjectDependencies(payload);
            }
        }

        public void CalculateVitals() {
            for (var i = 0; i < _groups.Length; i++)
                _groups[i].CalculateVitals();
        }

        public void ApplyVitals() {
            // First, calculate  vitals for every group
            for (var i = 0; i < _groups.Length; i++)
                _groups[i].ApplyVitals();

            // Afterwards, setup the joints of every bone
            for (var i = 0; i < _groups.Length; i++)
                _groups[i].ConfigureJoints();
        }

        public float GetTotalMass() {
            float mass = 0f;

            for (var i = 0; i < _groups.Length; i++)
                mass += _groups[i].GetTotalMass();

            return mass;
        }
    }
}
