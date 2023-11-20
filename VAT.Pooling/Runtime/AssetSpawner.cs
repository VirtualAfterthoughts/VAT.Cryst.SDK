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
        private struct SpawnConditions
        {
            public SpawnRules rules;

            public Vector3 position;

            public Quaternion rotation;

            public Vector3 scale;

            public AssetPoolableDelegate onSpawn;

            public SpawnConditions(SpawnRules rules, Vector3? position, Quaternion? rotation, Vector3? scale, AssetPoolableDelegate onSpawn)
            {
                this.rules = rules;
                this.position = position ?? Vector3.one;
                this.rotation = rotation ?? Quaternion.identity;
                this.scale = scale ?? Vector3.one;
                this.onSpawn = onSpawn;
            }
        }

        public static void Spawn(Address address, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, AssetPoolableDelegate onSpawn = null)
        {
            Spawn(address, SpawnRules.Default, position, rotation, scale, onSpawn);
        }

        public static void Spawn(Spawnable spawnable, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, AssetPoolableDelegate onSpawn = null)
        {
            Spawn(spawnable.contentReference.Address, spawnable.rules, position, rotation, scale, onSpawn);
        }

        public static void Spawn(Address address, SpawnRules rules, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, AssetPoolableDelegate onSpawn = null)
        {
            // Hook the PoolManager incase it hasn't initialized yet
            PoolManager.HookOnReady(() => { Internal_OnPoolManagerReady(address, rules, position, rotation, scale, onSpawn); });
        }

        private static void Internal_OnPoolManagerReady(Address address, SpawnRules rules, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, AssetPoolableDelegate onSpawn = null)
        {
            // Get the pool and wait for it to be ready
            if (PoolManager.Instance.FetchPool(address, out var pool))
            {
                var conditions = new SpawnConditions(rules, position, rotation, scale, onSpawn);

                pool.HookOnReady(() => { Internal_OnPoolReady(pool, conditions); });
            }
        }

        private static void Internal_OnPoolReady(AssetPool pool, SpawnConditions conditions)
        {
            // Now that we know the pool is ready, we can spawn our asset
            var poolable = pool.Spawn(conditions.rules, conditions.position, conditions.rotation, conditions.scale);

            // If we have a non-null result, it spawned successfully
            if (poolable != null)
            {
                conditions.onSpawn?.Invoke(poolable);
            }
        }
    }
}
