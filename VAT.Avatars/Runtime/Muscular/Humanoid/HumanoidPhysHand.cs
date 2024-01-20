using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Avatars.Constants;
using VAT.Avatars.REWORK;
using VAT.Cryst.Interfaces;

namespace VAT.Avatars.Muscular
{
    public class HumanoidPhysHand : HumanoidPhysBoneGroup, IHumanHand
    {
        public override int BoneCount => 1;

        public HumanoidPhysBone Hand => TBones[0];

        IBone IHumanHand.Hand => Hand;

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

        public override void Solve()
        {
            throw new System.NotImplementedException();
        }
    }
}
