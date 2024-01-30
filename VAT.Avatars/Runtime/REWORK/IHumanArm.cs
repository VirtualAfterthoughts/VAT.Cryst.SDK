using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.REWORK
{
    public interface IHumanArm : IArmGroup
    {
        public IBone Clavicle { get; }
        public IBone Scapula { get; }
        public IBone Wrist { get; }
        public IBone Carpal { get; }
    }
}
