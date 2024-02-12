using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VAT.Avatars.REWORK;

namespace VAT.Avatars
{
    public class RelativeThumb : IThumbGroup
    {
        public IBone Proximal => Bones[1];

        public IBone Middle => Bones[2];

        public IBone Distal => Bones[3];

        private readonly IBone[] _bones;
        public IBone[] Bones => _bones;

        public int BoneCount => _bones.Length;

        public IBoneGroup[] SubGroups => null;

        public int SubGroupCount => 0;

        public RelativeThumb(IBone parent, IBone targetParent, IThumbGroup targetThumb)
        {
            _bones = new IBone[targetThumb.BoneCount];
            for (var i = 0; i < BoneCount; i++)
            {
                _bones[i] = new RelativeBone(parent, targetParent, targetThumb.Bones[i]);
            }
        }
    }
}
