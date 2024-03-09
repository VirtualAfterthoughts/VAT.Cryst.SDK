using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Art
{
    [Serializable]
    public struct TransformArtReference : IArtReference
    {
        public Transform transform;

        [HideInInspector]
        public SimpleTransform offset;

        public readonly bool HasTransform {
            get {
                return transform != null;
            }
        }

        public SimpleTransform Transform
        {
            get
            {
                if (HasTransform)
                    return SimpleTransform.Create(transform);

                return SimpleTransform.Default;
            }
            set
            {
                if (HasTransform)
                    transform.SetPositionAndRotation(value.position, value.rotation);
            }
        }

        public SimpleTransform ArtOffset {
            readonly get { return offset; }
            set { offset = value; }
        }

        public TransformArtReference(Transform transform)
        {
            this.transform = transform;
            this.offset = SimpleTransform.Default;
        }
    }
}
