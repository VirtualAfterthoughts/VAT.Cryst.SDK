using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Entities.Stats;

namespace VAT.Avatars
{
    public interface IAvatarStats
    {
        public TStat GetStat<TStat>() where TStat : IStat
        {
            if (this is TStat stat)
            {
                return stat;
            }

            return default;
        }
    }
}
