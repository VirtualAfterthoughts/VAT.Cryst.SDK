using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Art
{
    public class HumanoidArtSpine : HumanoidArtBoneGroupT<HumanoidSpineDescriptor, HumanoidSpine, HumanoidPhysSpine>
    {
        public override int BoneCount => 4;

        public ArtBone UpperChest => Bones[0];
        public ArtBone Chest => Bones[1];
        public ArtBone Spine => Bones[2];
        public ArtBone Hips => Bones[3];

        public override void WriteOffsets(HumanoidSpine dataGroup) {
            UpperChest.WriteOffset(dataGroup.T1Vertebra);
            Chest.WriteOffset(dataGroup.T7Vertebra);
            Spine.WriteOffset(dataGroup.L1Vertebra);
            Hips.WriteOffset(dataGroup.Sacrum);
        }

        public override void WriteTransforms(HumanoidSpineDescriptor artDescriptorGroup) {
            UpperChest.WriteReference(artDescriptorGroup.upperChest);
            Chest.WriteReference(artDescriptorGroup.chest);
            Spine.WriteReference(artDescriptorGroup.spine);
            Hips.WriteReference(artDescriptorGroup.hips);
        }

        public override void Solve() {
            Hips.Solve(PhysGroup.Sacrum.Transform);
            Spine.Solve(PhysGroup.L1Vertebra.Transform);
            Chest.Solve(PhysGroup.T7Vertebra.Transform);
            UpperChest.Solve(PhysGroup.T1Vertebra.Transform);
        }
    }
}
