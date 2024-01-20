using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.REWORK;
using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Art
{
    public class HumanoidArtSpine : HumanoidArtBoneGroupT<HumanoidSpineDescriptor, IHumanSpine>
    {
        public override int BoneCount => 4;

        public ArtBone UpperChest => Bones[0] as ArtBone;
        public ArtBone Chest => Bones[1] as ArtBone;
        public ArtBone Spine => Bones[2] as ArtBone;
        public ArtBone Hips => Bones[3] as ArtBone;

        public override void WriteOffsets(IHumanSpine boneGroup) {
            UpperChest.WriteOffset(boneGroup.T1Vertebra);
            Chest.WriteOffset(boneGroup.T7Vertebra);
            Spine.WriteOffset(boneGroup.L1Vertebra);
            Hips.WriteOffset(boneGroup.Sacrum);
        }

        public override void WriteTransforms(HumanoidSpineDescriptor artDescriptorGroup) {
            UpperChest.WriteReference(artDescriptorGroup.upperChest);
            Chest.WriteReference(artDescriptorGroup.chest);
            Spine.WriteReference(artDescriptorGroup.spine);
            Hips.WriteReference(artDescriptorGroup.hips);
        }

        public override void Solve() {
            Hips.Solve(BoneGroup.Sacrum.Transform);
            Spine.Solve(BoneGroup.L1Vertebra.Transform);
            Chest.Solve(BoneGroup.T7Vertebra.Transform);
            UpperChest.Solve(BoneGroup.T1Vertebra.Transform);
        }
    }
}
