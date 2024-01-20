using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars
{
    public interface IJoint
    {
        SimpleTransform Transform { get; set; }
    }

    public struct BasicJoint : IJoint
    {
        public SimpleTransform Transform { get { return _transform; } set { _transform = value; } }

        private SimpleTransform _transform;

        public BasicJoint(SimpleTransform transform)
        {
            _transform = transform;
        }
    }
}
