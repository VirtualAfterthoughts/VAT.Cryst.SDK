using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Proportions;

using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Muscular
{
    public abstract class HumanoidPhysBoneGroup : PhysBoneGroupT<HumanoidPhysBone> {
        public void SetTransformRoot(Transform root) {
            for (var i = 0; i < BoneCount; i++)
                Bones[i].SetTransformRoot(root);

            for (var i = 0; i < SubGroupCount; i++) {
                if (SubGroups[i] is HumanoidPhysBoneGroup group)
                    group.SetTransformRoot(root);
            }
        }
    }
}
