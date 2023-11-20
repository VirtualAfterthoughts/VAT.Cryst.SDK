using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

namespace VAT.Pooling
{
    /// <summary>
    /// <para>Manager for all currently existing Asset Pools.</para>
    /// <para>For spawning assets, see <see cref="AssetSpawner"/>.</para>
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

        public bool FetchPool(Address address, out AssetPool pool)
        {
            pool = null;

            // If the pool already exists, we can just grab it from the dict
            if (_pools.ContainsKey(address))
            {
                pool = _pools[address];
                return true;
            }
            // Otherwise, we can attempt to create a new pool
            else if (AssetPackager.Instance.TryGetContent<ISpawnableContent>(address, out var content))
            {
                GameObject root = new($"Pool - {content.Info.Title}");
                pool = new AssetPool(content, root.transform);
                _pools.Add(address, pool);
                return true;
            }

            return false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void RuntimeInitialize()
        {
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
