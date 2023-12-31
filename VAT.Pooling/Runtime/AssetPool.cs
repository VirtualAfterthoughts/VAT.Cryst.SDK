using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;
using VAT.Shared.Extensions;

namespace VAT.Pooling
{
    public class AssetPool
    {
        private List<AssetPoolable> _poolables;

        private List<AssetPoolable> _spawnedPoolables;

        private List<AssetPoolable> _storedPoolables;

        private Transform _rootTransform;

        // The temp parent is used to keep an object disabled immediately upon its creation
        // This prevents events from being called when its not ready yet.
        private Transform _tempParent;

        private ISpawnableContent _content;

        private string _assetName;
        private Vector3 _assetScale;

        private long _lastId;
        private AssetPoolable _lastSpawned;

        public ulong LastID => (ulong)_lastId;
        public AssetPoolable LastSpawned => _lastSpawned;

        private bool _isReady = false;
        public bool IsReady => _isReady;

        private Action _onReady;

        public int SpawnedCount => _spawnedPoolables.Count;

        public AssetPool(ISpawnableContent content, Transform rootTransform = null)
        {
            _rootTransform = rootTransform;

            _tempParent = new GameObject("Temp Parent").transform;
            _tempParent.parent = _rootTransform;
            _tempParent.gameObject.SetActive(false);

            _content = content;

            _lastId = -1;
            _assetScale = Vector3.one;

            _poolables = new List<AssetPoolable>();
            _spawnedPoolables = new List<AssetPoolable>();
            _storedPoolables = new List<AssetPoolable>();

            if (_content.MainAssetT != null)
                _content.MainAssetT.LoadAsset(OnAssetLoaded);
        }

        public void HookOnReady(Action action)
        {
            if (_isReady)
            {
                action?.Invoke();
            }
            else
            {
                _onReady += action;
            }
        }

        private void OnAssetLoaded(GameObject asset)
        {
            _assetName = asset.name;
            _assetScale = asset.transform.localScale;

            _isReady = true;

            _onReady?.Invoke();
            _onReady = null;
        }

        public void Cleanup()
        {
            // Delete all poolables
            foreach (var poolable in _poolables)
            {
                if (poolable != null)
                {
                    GameObject.Destroy(poolable.gameObject);
                }
            }

            _poolables = null;
            _spawnedPoolables = null;
            _storedPoolables = null;

            // Delete the root
            if (_rootTransform != null)
            {
                GameObject.Destroy(_rootTransform.gameObject);
                _rootTransform = null;
            }

            // Delete the temp parent if it still exists
            if (_tempParent != null)
            {
                GameObject.Destroy(_tempParent.gameObject);
                _tempParent = null;
            }

            _content.MainAsset?.ReleaseAsset();
        }

        private Vector3 InternalEvaluateScale(Vector3? scale)
        {
            if (scale.HasValue)
                return scale.Value;
            else
                return _assetScale;
        }

        protected virtual AssetPoolable InternalInstantiate(bool isActive = false)
        {
            if (!_isReady)
                return null;

            GameObject go = GameObject.Instantiate(_content.MainAssetT.AssetT, _tempParent);

            AssetPoolable poolable = go.AddOrGetComponent<AssetPoolable>();
            poolable.InternalPoolSpawnDelegate = InternalOnSpawned;
            poolable.InternalPoolDespawnDelegate = InternalOnDespawned;

            go.SetActive(isActive);

            go.name = $"{_assetName} - [?]";
            return poolable;
        }

        private AssetPoolable InternalFetchPooled(SpawnRules rules)
        {
            switch (rules.spawnMode)
            {
                default:
                    for (var i = 0; i < _storedPoolables.Count; i++)
                    {
                        var stored = _storedPoolables[i];

                        if (stored.CanSpawn)
                        {
                            return stored;
                        }
                    }

                    break;
                case SpawnMode.REUSE_NEWEST:
                    for (var i = _storedPoolables.Count - 1; i > -1; i--)
                    {
                        var stored = _storedPoolables[i];

                        if (stored.CanSpawn)
                        {
                            return stored;
                        }
                    }
                    break;
            }

            return null;
        }

        private AssetPoolable InternalFetchSpawned(SpawnRules rules)
        {
            switch (rules.spawnMode)
            {
                default:
                    for (var i = 0; i < _spawnedPoolables.Count; i++)
                    {
                        var spawned = _spawnedPoolables[i];

                        if (spawned.CanSpawn)
                            return spawned;
                    }
                    break;
                case SpawnMode.REUSE_NEWEST:
                    for (var i = _spawnedPoolables.Count - 1; i > -1; i--)
                    {
                        var spawned = _spawnedPoolables[i];

                        if (spawned.CanDespawn)
                        {
                            return spawned;
                        }
                    }
                    break;
                case SpawnMode.REUSE_NONE:
                    return null;
            }
            return null;
        }

        private AssetPoolable InternalFetchNew()
        {
            var newPoolable = InternalInstantiate(false);

            if (newPoolable != null)
            {
                _poolables.Add(newPoolable);
            }

            return newPoolable;
        }

        private void InternalOnSpawned(AssetPoolable poolable)
        {
            InternalMoveToSpawned(poolable);
        }

        private void InternalOnDespawned(AssetPoolable poolable)
        {
            InternalMoveToDespawned(poolable);
        }

        private void InternalMoveToSpawned(AssetPoolable poolable)
        {
            // Clear it from existing lists
            _storedPoolables.Remove(poolable);
            _spawnedPoolables.Remove(poolable);

            // Move it to the front of the spawned list
            _spawnedPoolables.Add(poolable);
        }

        private void InternalMoveToDespawned(AssetPoolable poolable)
        {
            // Clear it from existing lists
            _spawnedPoolables.Remove(poolable);
            _storedPoolables.Remove(poolable);

            // Move it to the front of the stored list
            _storedPoolables.Add(poolable);
        }

        public void DespawnAll()
        {
            foreach (var poolable in _poolables)
                poolable.Despawn();
        }

        public AssetPoolable Spawn(SpawnRules rules, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null)
        {
            // Fetch the poolable
            AssetPoolable poolable;
            if (_storedPoolables.Count > 0)
                poolable = InternalFetchPooled(rules);
            else
            {
                if (SpawnedCount < rules.maxSpawned || rules.spawnMode == SpawnMode.GROW)
                {
                    poolable = InternalFetchNew();
                }
                else
                {
                    poolable = InternalFetchSpawned(rules);
                }
            }

            // Send over spawn events, and apply the transform to the poolable
            if (poolable != null)
            {
                // Make inactive
                poolable.gameObject.SetActive(false);

                // Update last values
                _lastId++;
                _lastSpawned = poolable;

                // Update name
                poolable.gameObject.name = $"{_assetName} - [{_lastId}]";

                // Use a null check for rotation, since the default value is a 0, 0, 0, 0 quaternion (not good)
                poolable.Transform.SetPositionAndRotation(position.GetValueOrDefault(), rotation ?? Quaternion.identity);

                poolable.Transform.localScale = InternalEvaluateScale(scale);

                // Move the location of the poolable in the lists
                InternalMoveToSpawned(poolable);

                // Invoke events
                poolable.OnSpawn((ulong)_lastId);

                // Set the poolable active
                poolable.Transform.parent = _rootTransform;
                poolable.gameObject.SetActive(true);
            }

            return poolable;
        }
    }
}
