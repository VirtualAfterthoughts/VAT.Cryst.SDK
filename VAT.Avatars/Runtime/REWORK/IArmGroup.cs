using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.REWORK
{
    public interface IArmGroup : IBoneGroup
    {
        public IBone UpperArm { get; }
        public IBone Elbow { get; }
        public IHandGroup Hand { get; }
    }
}
