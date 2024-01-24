using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Avatars.Constants;
using VAT.Avatars.REWORK;
using VAT.Cryst.Interfaces;

namespace VAT.Avatars.Muscular
{
    public class HumanoidPhysHand : HumanoidPhysBoneGroup, IHandGroup
    {
        public override int BoneCount => 1;

        public HumanoidPhysBone Hand => TBones[0];

        IBone IHandGroup.Hand => Hand;

        IBone IHandGroup.Palm => _relativePalm;
        private RelativeBone _relativePalm;

        public HumanoidPhysHand()
        {
            Initiate();
        }

        public HumanoidPhysHand(PhysBoneGroup parent) : this()
        {
            Attach(parent);
        }

        public override void Initiate()
        {
            base.Initiate();

            _bones[0] = new HumanoidPhysBone($"Hand", null, HumanoidConstants.HandLimits);
        }

        public void MatchPose(IHandGroup hand)
        {
            Hand.MatchBone(hand.Hand);

            _relativePalm = new RelativeBone(Hand, hand.Hand, hand.Palm);
        }

        public override void Solve()
        {
            throw new System.NotImplementedException();
        }
    }
}
