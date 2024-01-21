using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.REWORK;
using VAT.Shared.Data;

namespace VAT.Avatars
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

        private IBone _parent;
        private IBone _targetParent;
        private IBone _targetBone;

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
