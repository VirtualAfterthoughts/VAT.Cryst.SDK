using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Proportions;
using VAT.Avatars.REWORK;
using VAT.Avatars.Skeletal;
using VAT.Cryst;
using VAT.Input;
using VAT.Shared.Data;

namespace VAT.Avatars.Muscular
{
    public class HumanoidPhysSkeleton : PhysBoneSkeleton, IHumanSkeleton
    {
        private PhysBoneGroup[] _groups = null;
        public override PhysBoneGroup[] BoneGroups => _groups;

        public override int BoneGroupCount => 7;

        public HumanoidPhysNeck Neck => BoneGroups[0] as HumanoidPhysNeck;
        public HumanoidPhysSpine Spine => BoneGroups[1] as HumanoidPhysSpine;
        public HumanoidPhysArm LeftArm => BoneGroups[2] as HumanoidPhysArm;
        public HumanoidPhysArm RightArm => BoneGroups[3] as HumanoidPhysArm;
        public HumanoidPhysLeg LeftLeg => BoneGroups[4] as HumanoidPhysLeg;
        public HumanoidPhysLeg RightLeg => BoneGroups[5] as HumanoidPhysLeg;
        public PhysLocoLeg LocoLeg => BoneGroups[6] as PhysLocoLeg;

        IHumanNeck IHumanSkeleton.Neck => Neck;

        IHumanSpine IHumanSkeleton.Spine => Spine;

        IHumanArm IHumanSkeleton.LeftArm => LeftArm;

        IHumanArm IHumanSkeleton.RightArm => RightArm;

        IHumanLeg IHumanSkeleton.LeftLeg => LeftLeg;

        IHumanLeg IHumanSkeleton.RightLeg => RightLeg;

        IBoneGroup IHumanSkeleton.LocoLeg => LocoLeg;

        private IHumanSkeleton _skeleton = null;

        public override void Initiate()
        {
            _groups = new PhysBoneGroup[BoneGroupCount];
            _groups[0] = new HumanoidPhysNeck();
            _groups[1] = new HumanoidPhysSpine();
            _groups[2] = new HumanoidPhysArm();
            _groups[3] = new HumanoidPhysArm();
            _groups[4] = new HumanoidPhysLeg();
            _groups[5] = new HumanoidPhysLeg();
            _groups[6] = new PhysLocoLeg();

            LeftArm.isLeft = true;
            LeftLeg.isLeft = true;
        }

        public override SimpleTransform GetHead() {
            return Neck.GetHead();
        }

        public override void InitiateRuntime() {
            for (var i = 0; i < BoneGroupCount; i++) {
                _groups[i].Initiate();
            }

            Neck.Attach(Spine);

            LeftLeg.Attach(Spine);
            RightLeg.Attach(Spine);

            LeftArm.Attach(Spine);
            RightArm.Attach(Spine);

            LocoLeg.Attach(Spine);

            LocoLeg._pivot = Neck.Skull;
        }

        public void WriteProportions(HumanoidProportions proportions) {
            Neck.WriteProportions(proportions.neckProportions);
            Spine.WriteProportions(proportions.spineProportions, proportions.neckProportions);

            LeftLeg.WriteProportions(proportions.leftLegProportions);
            RightLeg.WriteProportions(proportions.rightLegProportions);

            LeftArm.WriteProportions(proportions.leftArmProportions);
            RightArm.WriteProportions(proportions.rightArmProportions);

            IgnoreCollisions(true);
        }

        public override void Solve()
        {
            base.Solve();

            if (_skeleton is HumanoidDataSkeleton temp)
            {
                temp.ShimbleWam = true;
                temp.Flof = Neck.Skull.Transform.InverseTransformPoint(GetFloor().position);
            }
        }

        public void WriteReferences(IHumanSkeleton skeleton)
        {
            LeftArm.Hand.MatchFingers(skeleton.LeftArm.Hand);
            RightArm.Hand.MatchFingers(skeleton.RightArm.Hand);
        }

        public void MatchPose(IHumanSkeleton skeleton) {
            _skeleton = skeleton;

            Spine.MatchPose(skeleton.Spine);
            Neck.MatchPose(skeleton.Neck);

            LeftArm.MatchPose(skeleton.LeftArm);
            RightArm.MatchPose(skeleton.RightArm);

            LeftLeg.MatchPose(skeleton.LeftLeg);
            RightLeg.MatchPose(skeleton.RightLeg);

            LocoLeg.MatchPose(skeleton.LocoLeg as LocoLeg);

            LocoLeg._pivotData = skeleton.Neck.Skull;

            ResetAnchors();
        }

        public void SetTransformRoot(Transform root) {
            for (var i = 0; i < BoneGroupCount; i++) {
                if (_groups[i] is HumanoidPhysBoneGroup group)
                    group.SetTransformRoot(root);
            }
        }

        public override bool TryGetHead(out PhysBone result)
        {
            result = Neck.Skull;
            return true;
        }

        public override bool TryGetHand(Handedness handedness, out PhysBone result)
        {
            switch (handedness) {
                default:
                    result = null;
                    return false;
                case Handedness.LEFT:
                    result = LeftArm.Hand.Hand;
                    return true;
                case Handedness.RIGHT:
                    result = RightArm.Hand.Hand;
                    return true;
            }
        }

        public override bool TryGetPelvis(out PhysBone result) {
            result = Spine.Sacrum;
            return true;
        }

        public override bool TryGetFoot(Handedness handedness, out PhysBone result) {
            switch (handedness) {
                default:
                    result = null;
                    return false;
                case Handedness.LEFT:
                    result = LeftLeg.Ankle;
                    return true;
                case Handedness.RIGHT:
                    result = RightLeg.Ankle;
                    return true;
            }
        }

        public override PhysBone GetRoot()
        {
            return Spine.Root;
        }

        public override SimpleTransform GetFloor() {
            return SimpleTransform.Create(LocoLeg.GetCenterOfPressure(), Spine.Root.Transform.rotation);
        }
    }
}
