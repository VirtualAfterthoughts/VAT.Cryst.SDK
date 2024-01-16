using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Art
{
    public abstract class ArtBoneGroup {
        public abstract void Initiate();

        public abstract void Solve();
    }

    public abstract class ArtBoneGroupT<TBone> : ArtBoneGroup where TBone : ArtBone {
        public abstract int BoneCount { get; }

        protected TBone[] _bones = null;
        public virtual TBone[] Bones => _bones;

        public override void Initiate() {
            _bones = new TBone[BoneCount];
        }
    }
}
