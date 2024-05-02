using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Bones
{
    public interface ILegGroup : ILimbGroup
    {
        IBone Hip { get; }
        IBone Knee { get; }
        IBone Ankle { get; }
        IBone Toe { get; }
    }
}
