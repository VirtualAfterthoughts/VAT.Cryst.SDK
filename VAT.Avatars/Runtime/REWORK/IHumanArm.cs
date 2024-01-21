using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.REWORK
{
    public interface IHumanArm : IBoneGroup
    {
        public IBone Clavicle { get; }
        public IBone Scapula { get; }
        public IBone UpperArm { get; }
        public IBone Elbow { get; }
        public IBone Wrist { get; }
        public IHumanHand Hand { get; }
    }
}
