using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Art
{
    public class HumanoidArtSkeleton : ArtBoneSkeletonT<HumanoidArtBoneGroup, HumanoidArtDescriptor, HumanoidDataSkeleton, HumanoidPhysSkeleton> {
        private HumanoidArtBoneGroup[] _groups = null;
        public override HumanoidArtBoneGroup[] BoneGroups => _groups;

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

        public override void WriteTransforms(HumanoidArtDescriptor artDescriptor) {
            Neck.WriteTransforms(artDescriptor.neckDescriptor);
            Spine.WriteTransforms(artDescriptor.spineDescriptor);

            LeftArm.WriteTransforms(artDescriptor.leftArmDescriptor);
            RightArm.WriteTransforms(artDescriptor.rightArmDescriptor);

            LeftLeg.WriteTransforms(artDescriptor.leftLegDescriptor);
            RightLeg.WriteTransforms(artDescriptor.rightLegDescriptor);
        }

        public override void WriteData(HumanoidDataSkeleton dataSkeleton, HumanoidPhysSkeleton physSkeleton)
        {
            Neck.WriteData(dataSkeleton.Neck, physSkeleton.Neck);
            Spine.WriteData(dataSkeleton.Spine, physSkeleton.Spine);

            LeftArm.WriteData(dataSkeleton.LeftArm, physSkeleton.LeftArm);
            RightArm.WriteData(dataSkeleton.RightArm, physSkeleton.RightArm);

            LeftLeg.WriteData(dataSkeleton.LeftLeg, physSkeleton.LeftLeg);
            RightLeg.WriteData(dataSkeleton.RightLeg, physSkeleton.RightLeg);
        }

        public override void WriteOffsets(HumanoidDataSkeleton dataSkeleton) {
            Neck.WriteOffsets(dataSkeleton.Neck);
            Spine.WriteOffsets(dataSkeleton.Spine);

            LeftArm.WriteOffsets(dataSkeleton.LeftArm);
            RightArm.WriteOffsets(dataSkeleton.RightArm);

            LeftLeg.WriteOffsets(dataSkeleton.LeftLeg);
            RightLeg.WriteOffsets(dataSkeleton.RightLeg);
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
