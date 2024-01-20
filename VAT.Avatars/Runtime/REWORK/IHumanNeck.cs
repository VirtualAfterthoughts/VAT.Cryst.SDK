using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.REWORK
{
    public interface IHumanNeck : IBoneGroup
    {
        public IBone C4Vertebra { get; }
        public IBone C1Vertebra { get; }
        public IBone Skull { get; }
    }
}
