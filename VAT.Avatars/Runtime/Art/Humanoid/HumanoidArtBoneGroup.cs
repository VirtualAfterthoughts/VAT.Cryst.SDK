using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Art
{
    public abstract class HumanoidArtBoneGroup : ArtBoneGroupT<ArtBone> {
        public override void Initiate() {
            base.Initiate();

            for (var i = 0; i < BoneCount; i++) {
                Bones[i] = new ArtBone();
            }
        }
    }

    public abstract class HumanoidArtBoneGroupT<TArtGroup, TDataGroup, TPhysGroup> : HumanoidArtBoneGroup
        where TArtGroup : IArtDescriptorGroup
        where TDataGroup : DataBoneGroup
        where TPhysGroup : PhysBoneGroup
    {
        protected TDataGroup _dataGroup;
        public TDataGroup DataGroup => _dataGroup;

        protected TPhysGroup _physGroup;
        public TPhysGroup PhysGroup => _physGroup;

        public abstract void WriteTransforms(TArtGroup artDescriptorGroup);

        public virtual void WriteData(TDataGroup dataGroup, TPhysGroup physGroup) {
            _dataGroup = dataGroup;
            _physGroup = physGroup;
        }

        public abstract void WriteOffsets(TDataGroup dataGroup);
    }
}
