using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Entities.Stats
{
    public interface IHealthStat : IStat
    {
        float Health { get; }
    }

    public interface IStrengthStat : IStat
    {
        float Strength { get; }
    }

    public interface ISpeedStat : IStat
    {
        float Speed { get; }
    }
}
