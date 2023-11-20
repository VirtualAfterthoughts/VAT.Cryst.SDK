using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    public class DynamicCrystAsset : IWeakAsset
    {
#if UNITY_EDITOR
        public Object EditorAsset => Asset;
#endif

        private readonly Object _internalAsset = null;
        private readonly bool _hasAsset;
        public virtual Object Asset => _internalAsset;

        public virtual Type AssetType => _internalAsset ? _internalAsset.GetType() : typeof(Object);

        public AssetLoadStatus Status
        {
            get
            {
                return _hasAsset ? AssetLoadStatus.DONE : AssetLoadStatus.FAILED;
            }
        }

        public DynamicCrystAsset() { }

        public DynamicCrystAsset(Object asset)
        {
            _internalAsset = asset;
            _hasAsset = true;
        }

        public void LoadAsset<T>(Action<T> onLoaded) where T : Object
        {
            if (Asset is T generic)
            {
                onLoaded(generic);
            }
        }

        public bool ReleaseAsset()
        {
            return false;
        }

        public void LoadScene(Action<SceneInstance> onLoaded, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            throw new NotImplementedException("Dynamic scene assets are not supported.");
        }

        public void UnloadScene(Action onSceneUnloaded)
        {
            throw new NotImplementedException("Dynamic scene assets are not supported.");
        }

        public UniTask<SceneInstance> LoadSceneAsync(LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            throw new NotImplementedException("Dynamic scene assets are not supported.");
        }

        public UniTask UnloadSceneAsync()
        {
            throw new NotImplementedException("Dynamic scene assets are not supported.");
        }
    }

    public class DynamicCrystAssetT<T> : DynamicCrystAsset, IWeakAssetT<T> where T : Object
    {
#if UNITY_EDITOR
        public T EditorAssetT => EditorAsset as T;
#endif

        private readonly T _internalAssetT = null;

        public override Object Asset => _internalAssetT;

        public override Type AssetType => typeof(T);

        public T AssetT => _internalAssetT;

        public DynamicCrystAssetT(Object asset)
        {
            _internalAssetT = (T)asset;
        }

        public DynamicCrystAssetT(T asset)
        {
            _internalAssetT = asset;
        }

        public void LoadAsset(Action<T> onLoaded) => base.LoadAsset<T>(onLoaded);
    }
}
