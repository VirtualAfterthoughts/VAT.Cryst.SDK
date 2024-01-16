using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using VAT.Avatars.Proportions;
using VAT.Avatars.Nervous;

using VAT.Input;

namespace VAT.Avatars.Skeletal
{
    public class HumanoidDataSkeleton : DataBoneSkeleton
    {
        private DataBoneGroup[] _groups = null;
        public override DataBoneGroup[] BoneGroups => _groups;

        public override int BoneGroupCount => 7;

        public HumanoidNeck Neck => BoneGroups[0] as HumanoidNeck;
        public HumanoidSpine Spine => BoneGroups[1] as HumanoidSpine;
        public HumanoidArm LeftArm => BoneGroups[2] as HumanoidArm;
        public HumanoidArm RightArm => BoneGroups[3] as HumanoidArm;
        public HumanoidLeg LeftLeg => BoneGroups[4] as HumanoidLeg;
        public HumanoidLeg RightLeg => BoneGroups[5] as HumanoidLeg;
        public LocoLeg LocoLeg => BoneGroups[6] as LocoLeg;

        public bool ShimbleWam;
        public float3 Flof;

        public override void Initiate()
        {
            _groups = new DataBoneGroup[BoneGroupCount];
            _groups[0] = new HumanoidNeck();
            _groups[1] = new HumanoidSpine();
            _groups[2] = new HumanoidArm();
            _groups[3] = new HumanoidArm();
            _groups[4] = new HumanoidLeg();
            _groups[5] = new HumanoidLeg();
            _groups[6] = new LocoLeg();

            for (var i = 0; i < BoneGroupCount; i++) {
                _groups[i].Initiate();
            }

            LeftArm.isLeft = true;
            LeftLeg.isLeft = true;

            Spine.Attach(Neck);
            LeftArm.Attach(Spine);
            RightArm.Attach(Spine);
            LeftLeg.Attach(Spine);
            RightLeg.Attach(Spine);
            LocoLeg.Attach(Spine);
        }

        private IAvatarPayload _payload;

        public override void Write(IAvatarPayload payload)
        {
            _payload = payload;

            base.Write(payload);
        }

        public void WriteProportions(HumanoidProportions proportions) {
            Neck.WriteProportions(proportions);
            Spine.WriteProportions(proportions);
            LeftArm.WriteProportions(proportions);
            RightArm.WriteProportions(proportions);
            LeftLeg.WriteProportions(proportions);
            RightLeg.WriteProportions(proportions);
            LocoLeg.WriteProportions(proportions.leftLegProportions.GetLength() + proportions.spineProportions.pelvisEllipsoid.height * 0.24f + proportions.leftLegProportions.ankleEllipsoid.height);
        }

        public override void Solve()
        {
            var start = _payload.GetRoot();
            _payload.TryGetHead(out var head);

            if (ShimbleWam) {
                var thing = head.TransformPoint(Flof);
                var rot = _payload.GetRoot();
                rot.position.y = thing.y;
                _payload.SetRoot(rot);
            }

            Neck.Solve();
            Spine.Solve();
            LeftArm.Solve();
            RightArm.Solve();
            LeftLeg.Solve();
            RightLeg.Solve();

            _payload.SetRoot(start);

            LocoLeg.Solve();
        }

        public override bool TryGetHead(out DataBone result)
        {
            result = Neck.Skull;
            return true;
        }

        public override bool TryGetHand(Handedness handedness, out DataBone result)
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

        public override bool TryGetPelvis(out DataBone result) {
            result = Spine.Sacrum;
            return true;
        }

        public override bool TryGetFoot(Handedness handedness, out DataBone result) {
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

        public override DataBone GetRoot()
        {
            return Spine.Root;
        }
    }
}
