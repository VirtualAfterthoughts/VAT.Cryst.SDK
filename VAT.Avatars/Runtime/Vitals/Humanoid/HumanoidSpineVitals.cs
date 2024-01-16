using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Proportions;

namespace VAT.Avatars.Vitals
{
    public class HumanoidSpineVitals : HumanoidBoneGroupVitals
    {
        public override int BoneCount => 4;

        private HumanoidSpineProportions _proportions;
        private HumanoidPhysSpine _spine;

        public float UpperChestMass => _boneMasses[0];
        public float ChestMass => _boneMasses[1];
        public float SpineMass => _boneMasses[2];
        public float PelvisMass => _boneMasses[3];

        public override void ConfigureJoints()
        {
            _spine.T1Vertebra.ConfigureJoint();
            _spine.T7Vertebra.ConfigureJoint();
            _spine.L1Vertebra.ConfigureJoint();
            _spine.Sacrum.ConfigureJoint();
        }

        public override void ApplyVitals()
        {
            // Apply mass
            _spine.T1Vertebra.SetMass(UpperChestMass);
            _spine.T7Vertebra.SetMass(ChestMass);
            _spine.L1Vertebra.SetMass(SpineMass);
            _spine.Sacrum.SetMass(PelvisMass);
        }

        public override void CalculateVitals()
        {
            // Calculate mass
            float totalVolume = _proportions.upperChestEllipsoid.GetVolume() + _proportions.chestEllipsoid.GetVolume() + _proportions.spineEllipsoid.GetVolume() + _proportions.pelvisEllipsoid.GetVolume();
            float weight = totalVolume * 1279.2602f;

            _boneMasses[0] = weight * 0.137100646f;
            _boneMasses[1] = weight * 0.20565097f;
            _boneMasses[2] = weight * 0.233610342f;
            _boneMasses[3] = weight * 0.273499538f;
        }

        protected override void OnInjectDependencies(HumanoidVitalsPayload payload)
        {
            _proportions = payload.Proportions.spineProportions;
            _spine = payload.Skeleton.Spine;
        }
    }
}
