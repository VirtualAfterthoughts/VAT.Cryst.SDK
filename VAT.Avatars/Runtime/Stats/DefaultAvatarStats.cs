using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Entities.Stats;

namespace VAT.Avatars
{
    public struct DefaultAvatarStats : IAvatarStats, IHealthStat, IStrengthStat, ISpeedStat
    {
        public float health;
        public float strength;
        public float speed;

        public float Health => health;

        public float Strength => strength;

        public float Speed => speed;
    }
}
