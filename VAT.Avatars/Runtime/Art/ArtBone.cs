using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.REWORK;
using VAT.Shared.Data;

namespace VAT.Avatars.Art
{
    public class ArtBone : IBone
    {
        private IArtReference _artReference = null;
        public IArtReference ArtReference => _artReference;

        private bool _hasReference = false;
        public bool HasReference => _hasReference;

        public SimpleTransform Transform {
            get { return ArtReference.Transform; }
            set { ArtReference.Transform = value; }
        }

        private SimpleTransform _defaultTransform = SimpleTransform.Default;

        IBone IBone.Parent => throw new System.NotImplementedException();

        int IBone.ChildCount => 0;

        public void Deinitiate()
        {
            if (HasReference)
            {
                ArtReference.LocalTransform = _defaultTransform;
            }
        }

        public void WriteReference(IArtReference reference) {
            _artReference = reference;
            _hasReference = true;

            _defaultTransform = reference.LocalTransform;
        }

        public void WriteOffset(IBone bone) {
            if (HasReference)
                ArtReference.WriteOffset(bone);
        }

        public void Solve(SimpleTransform target) {
            if (HasReference)
                Transform = target.Transform(ArtReference.ArtOffset);
        }

        IBone IBone.GetChild(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}
