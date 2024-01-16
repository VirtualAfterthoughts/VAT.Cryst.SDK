using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Art
{
    [Serializable]
    public sealed class TransformArtReference : IArtReference
    {
        public Transform transform;

        [HideInInspector]
        public SimpleTransform offset = SimpleTransform.Default;

        public bool HasTransform {
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
            get { return offset; }
            set { offset = value; }
        }

        public static implicit operator TransformArtReference(Transform transform) {
            return new TransformArtReference() {
                transform = transform
            };
        }

        public static implicit operator bool(TransformArtReference reference) {
            return reference != null && reference.HasTransform;
        }
    }
}
