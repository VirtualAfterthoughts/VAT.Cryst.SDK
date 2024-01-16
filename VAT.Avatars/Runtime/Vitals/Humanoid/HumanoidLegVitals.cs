using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Proportions;

namespace VAT.Avatars.Vitals
{
    public class HumanoidLegVitals : HumanoidBoneGroupVitals
    {
        public override int BoneCount => 3;

        private readonly bool _isLeft;

        private HumanoidLegProportions _proportions;
        private HumanoidPhysLeg _leg;

        public float HipMass => _boneMasses[0];
        public float KneeMass => _boneMasses[1];
        public float AnkleMass => _boneMasses[2];

        public HumanoidLegVitals(bool isLeft)
        {
            _isLeft = isLeft;
        }


        public override void ConfigureJoints()
        {
            _leg.Hip.ConfigureJoint();
            _leg.Knee.ConfigureJoint();
            _leg.Ankle.ConfigureJoint();
        }

        public override void ApplyVitals()
        {
            // Apply mass
            _leg.Hip.SetMass(HipMass);
            _leg.Knee.SetMass(KneeMass);
            _leg.Ankle.SetMass(AnkleMass);
        }

        public override void CalculateVitals()
        {
            // Calculate mass
            float density = 1.5f * 1000f;

            _boneMasses[0] = Internal_CalculateMass(_proportions.hipEllipsoid, density * 0.95f);
            _boneMasses[1] = Internal_CalculateMass(_proportions.kneeEllipsoid, density * 0.7f);
            _boneMasses[2] = Internal_CalculateMass(_proportions.ankleEllipsoid, density * 2f);
        }

        protected override void OnInjectDependencies(HumanoidVitalsPayload payload)
        {
            if (_isLeft)
            {
                _proportions = payload.Proportions.leftLegProportions;
                _leg = payload.Skeleton.LeftLeg;
            }
            else
            {
                _proportions = payload.Proportions.rightLegProportions;
                _leg = payload.Skeleton.RightLeg;
            }
        }
    }
}
