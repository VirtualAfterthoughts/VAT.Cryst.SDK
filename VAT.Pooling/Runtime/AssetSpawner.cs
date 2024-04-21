using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

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

        public static void Register(Spawnable spawnable)
        {
            PoolManager.HookOnReady(() =>
            {
                PoolManager.Instance.CreatePool(spawnable.contentReference.Address);
            });
        }

        public static void Spawn(SpawnRequestInfo info)
        {
            PoolManager.HookOnReady(() => 
            { 
                OnPoolManagerReady(info); 
            });
        }

        private static void OnPoolManagerReady(SpawnRequestInfo info)
        {
            // Get the pool and wait for it to be ready
            var (exists, pool) = PoolManager.Instance.FetchPool(info.spawnable.contentReference.Address); ;

            if (exists)
            {
                pool.HookOnReady(() => { OnPoolReady(pool, info); });
            }
            else
            {
                Debug.LogWarning($"Tried spawning a spawnable at address {info.spawnable.contentReference.Address}, but the pool hasn't been registered!");
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
