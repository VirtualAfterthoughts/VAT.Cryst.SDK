using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Avatars.Constants;
using VAT.Avatars.REWORK;
using VAT.Cryst.Interfaces;
using VAT.Shared.Data;

namespace VAT.Avatars.Muscular
{
    public class HumanoidPhysHand : HumanoidPhysBoneGroup, IHandGroup
    {
        public override int BoneCount => 1;

        public HumanoidPhysBone Hand => TBones[0];

        IBone IHandGroup.Hand => Hand;

        IBone IHandGroup.Palm => _relativePalm;
        private RelativeBone _relativePalm;

        private IHandGroup _hand;

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
            _hand = hand;

            Hand.MatchBone(hand.Hand);

            _relativePalm = new RelativeBone(Hand, hand.Hand, hand.Palm);
        }

        public override void Solve()
        {
            throw new System.NotImplementedException();
        }

        public SimpleTransform GetPointOnPalm(Vector2 position)
        {
            return _relativePalm.Transform.Transform(_hand.Palm.Transform.InverseTransform(_hand.GetPointOnPalm(position)));
        }
    }
}
