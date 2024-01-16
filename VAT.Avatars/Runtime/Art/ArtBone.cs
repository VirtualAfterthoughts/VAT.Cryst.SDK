using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Art
{
    public class ArtBone : IBone {
        private IArtReference _artReference = null;
        public IArtReference ArtReference => _artReference;

        private bool _hasReference = false;
        public bool HasReference => _hasReference;

        public SimpleTransform Transform {
            get { return ArtReference.Transform; }
            set { ArtReference.Transform = value; }
        }

        public void WriteReference(IArtReference reference) {
            _artReference = reference;
            _hasReference = true;
        }

        public void WriteOffset(IBone bone) {
            if (HasReference)
                ArtReference.WriteOffset(bone);
        }

        public void Solve(SimpleTransform target) {
            if (HasReference)
                Transform = target.Transform(ArtReference.ArtOffset);
        }
    }
}
