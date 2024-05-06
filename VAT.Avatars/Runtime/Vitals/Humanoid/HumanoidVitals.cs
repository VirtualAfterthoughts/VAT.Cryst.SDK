using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars.Proportions;

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

        public DefaultAvatarStats GetStats(HumanoidProportions proportions)
        {
            float chestCircumference = proportions.GetChestCircumference();
            float health = Mathf.RoundToInt(chestCircumference * 100f);

            float legLength = proportions.leftLegProportions.GetLength();

            var hipRadius = proportions.leftLegProportions.hipEllipsoid.radius;
            var calfRadius = proportions.leftLegProportions.kneeEllipsoid.radius;
            float hipCircumference = proportions.GetCircumference(hipRadius.x, hipRadius.y);
            float calfCircumference = proportions.GetCircumference(calfRadius.x, calfRadius.y);

            float calfStrength = calfCircumference / hipCircumference;
            float speed = calfStrength * legLength * 6f;

            var armProportions = proportions.leftArmProportions;
            var bicepRadius = (armProportions.upperArmEllipsoid.radius + armProportions.elbowEllipsoid.radius) / 2f;
            var bicepCircumference = proportions.GetCircumference(bicepRadius.x, bicepRadius.y);

            float strength = chestCircumference * bicepCircumference * 4f;

            return new DefaultAvatarStats()
            {
                health = health,
                speed = speed,
                strength = strength,
            };
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
