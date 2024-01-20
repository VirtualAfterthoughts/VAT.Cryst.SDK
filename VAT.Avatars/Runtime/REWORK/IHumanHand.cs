using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Avatars.REWORK
{
    public interface IHumanHand : IBoneGroup
    {
        public IBone Hand { get; }
    }
}
