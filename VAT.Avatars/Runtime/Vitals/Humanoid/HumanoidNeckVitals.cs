using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Proportions;

namespace VAT.Avatars.Vitals
{
    public class HumanoidNeckVitals : HumanoidBoneGroupVitals
    {
        public override int BoneCount => 3;

        private HumanoidNeckProportions _proportions;
        private HumanoidPhysNeck _neck;

        public float SkullMass => _boneMasses[0];
        public float UpperNeckMass => _boneMasses[1];
        public float LowerNeckMass => _boneMasses[2];

        public override void ConfigureJoints()
        {
            _neck.Skull.ConfigureJoint();
            _neck.C1Vertebra.ConfigureJoint();
            _neck.C4Vertebra.ConfigureJoint();
        }

        public override void ApplyVitals()
        {
            // Apply mass
            _neck.Skull.SetMass(SkullMass);
            _neck.C1Vertebra.SetMass(UpperNeckMass);
            _neck.C4Vertebra.SetMass(LowerNeckMass);
        }

        public override void CalculateVitals()
        {
            // Calculate mass
            float totalVolume = _proportions.skullEllipsoid.GetVolume() + _proportions.upperNeckEllipsoid.GetVolume() + _proportions.lowerNeckEllipsoid.GetVolume();
            float weight = totalVolume * 810.678542f;

            _boneMasses[0] = weight * 0.2f;
            _boneMasses[1] = weight * 0.3f;
            _boneMasses[2] = weight * 0.5f;
        }

        protected override void OnInjectDependencies(HumanoidVitalsPayload payload)
        {
            _proportions = payload.Proportions.neckProportions;
            _neck = payload.Skeleton.Neck;
        }
    }
}
