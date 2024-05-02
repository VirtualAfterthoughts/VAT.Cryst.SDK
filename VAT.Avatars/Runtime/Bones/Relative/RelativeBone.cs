using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Bones
{
    public class RelativeBone : IBone
    {
        public SimpleTransform Transform
        {
            get
            {
                return _parent.TransformBone(_targetParent, _targetBone);
            }
        }

        public IBone Parent => null;

        public int ChildCount => 0;

        private readonly IBone _parent;
        private readonly IBone _targetParent;
        private readonly IBone _targetBone;

        public RelativeBone(IBone parent, IBone targetParent, IBone targetBone)
        {
            _parent = parent;
            _targetParent = targetParent;
            _targetBone = targetBone;
        }

        public IBone GetChild(int index)
        {
            throw new IndexOutOfRangeException("RelativeBones have no children!");
        }
    }
}
