using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    public static class AssetUnlocker
    {
        public static bool IsUnlocked(IShard shard)
        {
            return !shard.ShardInfo.Unlockable;
        }

        public static void UnlockShard(IShard shard)
        {
        }

        public static void LockShard(IShard shard)
        {
        }
    }
}
