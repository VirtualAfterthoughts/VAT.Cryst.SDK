using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Skeletal
{
    public class DataArm : DataBoneGroup
    {
        public DataBone UpperArm => Bones[0];
        public DataBone Elbow => Bones[1];
        public DataBone Wrist => Bones[2];
        public DataBone Carpal => Bones[3];
        public DataBone Hand => Bones[4];

        public override int BoneCount => 5;

        public override void Solve()
        {
        }
    }
}
