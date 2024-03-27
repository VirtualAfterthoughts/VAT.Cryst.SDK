using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.REWORK;
using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Art
{
    public class HumanoidArtSkeleton : ArtBoneSkeletonT<HumanoidArtDescriptor, IHumanSkeleton> {
        private HumanoidArtBoneGroup[] _groups = null;
        public override IBoneGroup[] BoneGroups => _groups;

        public override int BoneGroupCount => 6;

        public HumanoidArtNeck Neck => BoneGroups[0] as HumanoidArtNeck;
        public HumanoidArtSpine Spine => BoneGroups[1] as HumanoidArtSpine;
        public HumanoidArtArm LeftArm => BoneGroups[2] as HumanoidArtArm;
        public HumanoidArtArm RightArm => BoneGroups[3] as HumanoidArtArm;
        public HumanoidArtLeg LeftLeg => BoneGroups[4] as HumanoidArtLeg;
        public HumanoidArtLeg RightLeg => BoneGroups[5] as HumanoidArtLeg;

        public override void Initiate()
        {
            _groups = new HumanoidArtBoneGroup[BoneGroupCount];
            _groups[0] = new HumanoidArtNeck();
            _groups[1] = new HumanoidArtSpine();
            _groups[2] = new HumanoidArtArm();
            _groups[3] = new HumanoidArtArm();
            _groups[4] = new HumanoidArtLeg();
            _groups[5] = new HumanoidArtLeg();

            for (var i = 0; i < BoneGroupCount; i++) {
                _groups[i].Initiate();
            }
        }

        public override void Deinitiate()
        {
            foreach (var group in _groups)
            {
                group.Deinitiate();
            }
        }

        public override void WriteTransforms(HumanoidArtDescriptor artDescriptor) {
            Neck.WriteTransforms(artDescriptor.neckDescriptor);
            Spine.WriteTransforms(artDescriptor.spineDescriptor);

            LeftArm.WriteTransforms(artDescriptor.leftArmDescriptor);
            RightArm.WriteTransforms(artDescriptor.rightArmDescriptor);

            LeftLeg.WriteTransforms(artDescriptor.leftLegDescriptor);
            RightLeg.WriteTransforms(artDescriptor.rightLegDescriptor);
        }

        public override void WriteData(IHumanSkeleton skeleton)
        {
            Neck.WriteData(skeleton.Neck);
            Spine.WriteData(skeleton.Spine);

            LeftArm.WriteData(skeleton.LeftArm);
            RightArm.WriteData(skeleton.RightArm);

            LeftLeg.WriteData(skeleton.LeftLeg);
            RightLeg.WriteData(skeleton.RightLeg);
        }

        public override void WriteOffsets(IHumanSkeleton skeleton) {
            Neck.WriteOffsets(skeleton.Neck);
            Spine.WriteOffsets(skeleton.Spine);

            LeftArm.WriteOffsets(skeleton.LeftArm);
            RightArm.WriteOffsets(skeleton.RightArm);

            LeftLeg.WriteOffsets(skeleton.LeftLeg);
            RightLeg.WriteOffsets(skeleton.RightLeg);
        }

        public override void Solve() {
            Spine.Solve();

            LeftLeg.Solve();
            RightLeg.Solve();

            LeftArm.Solve();
            RightArm.Solve();

            Neck.Solve();
        }
    }
}
