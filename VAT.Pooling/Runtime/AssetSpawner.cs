using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Pooling
{
    /// <summary>
    /// Utility class for spawning assets based on their asset address.
    /// </summary>
    public static class AssetSpawner
    {
        public struct SpawnRequestInfo
        {
            public Vector3? position;

            public Quaternion? rotation;

            public Vector3? scale;

            public Spawnable spawnable;

            public Action<SpawnCallbackInfo> spawnCallback;
        }

        public struct SpawnCallbackInfo
        {
            public AssetPoolable assetPoolable;
        }

        public static void Spawn(SpawnRequestInfo info)
        {
            // Hook the PoolManager incase it hasn't initialized yet
            PoolManager.HookOnReady(() => { OnPoolManagerReady(info); });
        }

        private static void OnPoolManagerReady(SpawnRequestInfo info)
        {
            // Get the pool and wait for it to be ready
            if (PoolManager.Instance.FetchPool(info.spawnable.contentReference.Address, out var pool))
            {
                pool.HookOnReady(() => { OnPoolReady(pool, info); });
            }
        }

        private static void OnPoolReady(AssetPool pool, SpawnRequestInfo info)
        {
            // Now that we know the pool is ready, we can spawn our asset
            var poolable = pool.Spawn(info.spawnable.rules, info.position, info.rotation, info.scale);

            // If we have a non-null result, it spawned successfully
            if (poolable != null)
            {
                info.spawnCallback?.Invoke(new SpawnCallbackInfo()
                {
                    assetPoolable = poolable,
                });
            }
        }
    }
}
