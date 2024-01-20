using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

using VAT.Avatars.REWORK;

namespace VAT.Avatars.Art
{
    public interface IArtReference {
        public abstract bool HasTransform { get; }

        public abstract SimpleTransform Transform { get; set; }

        public SimpleTransform ArtOffset { get; set; }

        public void WriteOffset(IBone bone) {
            if (HasTransform)
                ArtOffset = bone.Transform.InverseTransform(Transform);
        }
    }
}
