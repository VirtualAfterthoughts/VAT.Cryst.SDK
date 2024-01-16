using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars
{
    public static partial class IBoneExtensions {
        public static SimpleTransform TransformBone(this IBone self, IBone parent, IBone bone) {
            return self.Transform.Transform(parent.Transform.InverseTransform(bone.Transform));
        }
    }

    public interface IBone {
        public SimpleTransform Transform { get; set; }
    }
}
