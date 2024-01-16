using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Art
{
    public class HumanoidArtNeck : HumanoidArtBoneGroupT<HumanoidNeckDescriptor, HumanoidNeck, HumanoidPhysNeck>
    {
        public override int BoneCount => 3;

        public ArtBone Head => Bones[0];
        public ArtBone UpperNeck => Bones[1];
        public ArtBone LowerNeck => Bones[2];

        public override void Solve()
        {
            LowerNeck.Solve(PhysGroup.C4Vertebra.Transform);
            UpperNeck.Solve(PhysGroup.C1Vertebra.Transform);
            Head.Solve(PhysGroup.Skull.Transform);
        }

        public override void WriteOffsets(HumanoidNeck dataGroup) {
            Head.WriteOffset(dataGroup.Skull);
            UpperNeck.WriteOffset(dataGroup.C1Vertebra);
            LowerNeck.WriteOffset(dataGroup.C4Vertebra);
        }

        public override void WriteTransforms(HumanoidNeckDescriptor artDescriptorGroup)
        {
            Head.WriteReference(artDescriptorGroup.head);
            UpperNeck.WriteReference(artDescriptorGroup.upperNeck);
            LowerNeck.WriteReference(artDescriptorGroup.lowerNeck);
        }
    }
}
