using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Cryst;

using VAT.Shared;
using VAT.Shared.Data;
using VAT.Shared.Extensions;
using VAT.Shared.Utilities;

namespace VAT.Pooling
{
    public delegate void AssetPoolableDelegate(AssetPoolable poolable);
    public delegate void AssetSpawnDelegate(AssetPoolable poolable, ulong id);

    public class AssetPoolable : CachedMonoBehaviour, IDespawnable, IRespawnable
    {
        public static ComponentCache<AssetPoolable> Cache { get; private set; } = new ComponentCache<AssetPoolable>();

        internal AssetPoolableDelegate InternalPoolSpawnDelegate { get; set; }
        internal AssetPoolableDelegate InternalPoolDespawnDelegate { get; set; }

        public event AssetSpawnDelegate OnSpawnDelegate;
        public event AssetPoolableDelegate OnDespawnDelegate;

        private bool _isLocked = false;
        public bool IsLocked => _isLocked;

        private Transform _initialParent;
        private SimpleTransform _spawnTransform = SimpleTransform.Default;

        public virtual bool CanSpawn
        {
            get { return !_isLocked; }
        }

        public virtual bool CanDespawn
        {
            get { return true; }
        }

        private ulong _id;
        public ulong ID => _id;

        private void Awake()
        {
            Cache.Add(gameObject, this);
            IDespawnable.Cache.Add(gameObject, this);
            IRespawnable.Cache.Add(gameObject, this);

            _initialParent = Transform.parent;
            _spawnTransform = Transform;
        }

        private void OnDestroy()
        {
            Cache.Remove(gameObject, this);
            IDespawnable.Cache.Remove(gameObject, this);
            IRespawnable.Cache.Remove(gameObject, this);
        }

#if UNITY_EDITOR
        [ContextMenu("Respawn")]
#endif
        public void Respawn()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            if (!CanDespawn)
                return;

            OnDespawn();

            Transform.SetPositionAndRotation(_spawnTransform.position, _spawnTransform.rotation);

            GameObject.SetActive(true);
            OnSpawn(_id);
        }

        internal virtual void OnSpawn(ulong id)
        {
            _id = id;
            OnSpawnDelegate?.Invoke(this, id);
            InternalPoolSpawnDelegate?.Invoke(this);

            _spawnTransform = Transform;
        }

#if UNITY_EDITOR
        [ContextMenu("Despawn")]
#endif
        /// <summary>
        /// Despawns the poolable.
        /// </summary>
        public void Despawn()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            if (!CanDespawn)
                return;

            OnDespawn();
        }

        internal virtual void OnDespawn()
        {
            _id = 0;
            gameObject.SetActive(false);
            OnDespawnDelegate?.Invoke(this);
            InternalPoolDespawnDelegate?.Invoke(this);

            Transform.EnsureParent(_initialParent);
        }

        /// <summary>
        /// Locks the poolable, preventing it from being reused.
        /// </summary>
        public void Lock()
        {
            _isLocked = true;
        }

        /// <summary>
        /// Unlocks the poolable and allows it to be reused.
        /// </summary>
        public void Unlock()
        {
            _isLocked = false;
        }

        /// <summary>
        /// Flags this poolable so that it is counted as "despawned" and may be reused.
        /// </summary>
        public void FlagForRespawning()
        {
            InternalPoolDespawnDelegate?.Invoke(this);
        }

        /// <summary>
        /// Flags this poolable so that it is counted as "spawned".
        /// </summary>
        public void FlagForDespawning()
        {
            InternalPoolSpawnDelegate?.Invoke(this);
        }
    }
}
