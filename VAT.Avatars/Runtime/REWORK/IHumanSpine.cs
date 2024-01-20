using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Avatars.REWORK
{
    public interface IHumanSpine : IBoneGroup
    {
        public IBone Root { get; }
        public IBone Sacrum { get; }
        public IBone L1Vertebra { get; }
        public IBone T7Vertebra { get; }
        public IBone T1Vertebra { get; }
    }
}
