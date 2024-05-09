using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Art
{
    [Serializable]
    public class TransformArtReference : IArtReference
    {
        public Transform transform;

        [HideInInspector]
        public SimpleTransform offset;

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
                    return SimpleTransform.Create(transform.position, transform.rotation);

                return SimpleTransform.Default;
            }
            set
            {
                if (HasTransform)
                    transform.SetPositionAndRotation(value.position, value.rotation);
            }
        }

        public SimpleTransform LocalTransform
        {
            get
            {
                if (HasTransform)
                {
                    return SimpleTransform.Create(transform.localPosition, transform.localRotation);
                }

                return SimpleTransform.Default;
            }
            set
            {
                if (HasTransform)
                {
                    transform.SetLocalPositionAndRotation(value.position, value.rotation);
                }
            }
        }

        public SimpleTransform ArtOffset {
            get { return offset; }
            set { offset = value; }
        }

        public TransformArtReference(Transform transform)
        {
            this.transform = transform;
            this.offset = SimpleTransform.Default;
        }
    }
}
