using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

namespace VAT.Pooling
{
    /// <summary>
    /// <para>Manager for all currently existing Asset Pools.</para>
    /// <para>For spawning assets, see <see cref="GlobalSpawner"/>.</para>
    /// </summary>
    public class PoolManager
    {
        private static PoolManager _instance;
        public static PoolManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public PoolManager(bool init = true)
        {
            if (init)
            {
                _pools = new Dictionary<string, AssetPool>();
            }
        }

        private void OnPreLevelLoad()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Cleanup();
            }

            _pools.Clear();
        }

        private Dictionary<string, AssetPool> _pools;

        private static Action _onReady;
        public static bool IsReady { get; private set; }

        public static void HookOnReady(Action action)
        {
            if (IsReady)
            {
                action?.Invoke();
            }
            else
            {
                _onReady += action;
            }
        }

        public IReadOnlyCollection<AssetPool> GetPools()
        {
            return _pools.Values;
        }

        public (bool success, AssetPool pool) CreatePool(Address address)
        {
            var fetched = FetchPool(address);

            if (fetched.exists)
            {
                return (false, fetched.pool);
            }

            if (AssetPackager.Instance.TryGetShard<ISpawnableShard>(address, out var content))
            {
                GameObject root = new($"Pool - {content.Info.Title}");
                var pool = new AssetPool(content, root.transform);
                _pools.Add(address.ID, pool);
                return (true, pool);
            }

            return (false, null);
        }

        public (bool exists, AssetPool pool) FetchPool(Address address)
        {
            // If the pool already exists, we can just grab it from the dict
            if (_pools.ContainsKey(address.ID))
            {
                var pool = _pools[address.ID];
                return (true, pool);
            }

            return (false, null);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void RuntimeInitialize()
        {
            _instance = null;
            IsReady = false;

            AssetPackager.HookOnReady(OnAssetPackagerReady);
        }

        private static void OnAssetPackagerReady()
        {
            _instance = new PoolManager();
            IsReady = true;

            _onReady?.Invoke();
            _onReady = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LevelInitialize()
        {
            HookOnReady(() => { _instance.OnPreLevelLoad(); });
        }
    }
}
