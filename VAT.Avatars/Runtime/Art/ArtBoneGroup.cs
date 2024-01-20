using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars.REWORK;

namespace VAT.Avatars.Art
{
    public abstract class ArtBoneGroup : IBoneGroup {
        public abstract IBone[] Bones { get; }
        public abstract int BoneCount { get; }
        public abstract IBoneGroup[] SubGroups { get; }
        public abstract int SubGroupCount { get; }

        public abstract void Initiate();

        public abstract void Solve();
    }

    public abstract class ArtBoneGroupT<TBone> : ArtBoneGroup where TBone : ArtBone {
        public sealed override IBone[] Bones => TBones;
        public override int SubGroupCount => 0;
        public override IBoneGroup[] SubGroups => null;

        protected TBone[] _bones = null;
        public virtual TBone[] TBones => _bones;

        public override void Initiate() {
            _bones = new TBone[BoneCount];
        }
    }
}
