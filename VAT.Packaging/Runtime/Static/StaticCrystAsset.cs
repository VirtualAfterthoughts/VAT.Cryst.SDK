using Cysharp.Threading.Tasks;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    [Serializable]
    public class StaticCrystAsset : IWeakAsset
    {
#if UNITY_EDITOR
        protected Object _editorAsset;
        public virtual Object EditorAsset { get { return _editorAsset; } set { _editorAsset = value; } }
#endif

        public virtual Object Asset
        {
            get
            {
                if (!OperationHandle.IsValid())
                    return null;

                return OperationHandle.Result as Object;
            }
        }

        private Type _assetType;
        public virtual Type AssetType
        {
            get
            {
                if (_assetType == null)
                    return typeof(Object);

                return _assetType;
            }
        }

        [SerializeField]
        [HideInInspector]
        private string _guid;
        public string AssetGUID => _guid;

        private AsyncOperationHandle _operationHandle;
        public AsyncOperationHandle OperationHandle => _operationHandle;

        public AssetLoadStatus Status
        {
            get
            {
                if (!OperationHandle.IsValid())
                    return AssetLoadStatus.IDLE;

                return OperationHandle.Status switch
                {
                    AsyncOperationStatus.Failed => AssetLoadStatus.FAILED,

                    AsyncOperationStatus.Succeeded => AssetLoadStatus.DONE,

                    _ => AssetLoadStatus.LOADING,
                };
            }
        }

        public StaticCrystAsset(string guid)
        {
            _guid = guid;
        }

        public StaticCrystAsset()
        {

        }

#if UNITY_EDITOR
        public void ValidateGUID(Object editorAsset)
        {
            EditorAsset = editorAsset;

            if (editorAsset == null)
            {
                _guid = null;
                return;
            }

            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(EditorAsset, out string guid, out long _))
                _guid = guid;
        }

        public void ValidateGUID(string guid)
        {
            _guid = guid;
            ValidateGUID();
        }

        public void ValidateGUID()
        {
            string path = AssetDatabase.GUIDToAssetPath(AssetGUID);
            if (!string.IsNullOrEmpty(path))
                EditorAsset = AssetDatabase.LoadAssetAtPath(path, AssetType);
            else
                EditorAsset = null;
        }
#endif

        public void LoadAsset<T>(Action<T> onLoaded) where T : Object
        {
            Internal_LoadAsset(onLoaded).Forget();
        }

        protected async virtual UniTaskVoid Internal_LoadAsset<T>(Action<T> onLoaded) where T : Object
        {
            var asset = await LoadAssetAsync<T>();
            onLoaded?.Invoke(asset);
        }

        public async UniTask<T> LoadAssetAsync<T>() where T : Object
        {
            switch (Status)
            {
                case AssetLoadStatus.DONE:
                case AssetLoadStatus.LOADING:
                    return await OperationHandle.Convert<T>().ToUniTask();

                default:
                    var task = Internal_LoadAssetAsync<T>();
                    var result = await task;

                    _assetType = result.GetType();
                    return result;
            }
        }

        protected virtual AsyncOperationHandle<T> Internal_LoadAssetAsync<T>() where T : Object
        {
            AsyncOperationHandle<T> result = default;
            if (!_operationHandle.IsValid())
            {
                result = Addressables.LoadAssetAsync<T>(AssetGUID);
                _operationHandle = result;
            }

            return result;
        }

        public void LoadScene(Action<SceneInstance> onLoaded, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            Internal_LoadScene(onLoaded, mode, activateOnLoad).Forget();
        }

        protected async virtual UniTaskVoid Internal_LoadScene(Action<SceneInstance> onLoaded, LoadSceneMode mode, bool activateOnLoad)
        {
            var scene = await LoadSceneAsync(mode, activateOnLoad);
            onLoaded?.Invoke(scene);
        }

        public async UniTask<SceneInstance> LoadSceneAsync(LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            switch (Status)
            {
                case AssetLoadStatus.DONE:
                case AssetLoadStatus.LOADING:
                    return await OperationHandle.Convert<SceneInstance>().ToUniTask();

                default:
                    var task = Internal_LoadSceneAsync(mode, activateOnLoad);
                    var result = await task;

                    _assetType = result.GetType();
                    return result;
            }
        }

        protected virtual AsyncOperationHandle<SceneInstance> Internal_LoadSceneAsync(LoadSceneMode mode, bool activateOnLoad)
        {
            AsyncOperationHandle<SceneInstance> result = default;
            if (!_operationHandle.IsValid())
            {
                result = Addressables.LoadSceneAsync(AssetGUID, mode, activateOnLoad);
                _operationHandle = result;
            }

            return result;
        }

        public virtual bool ReleaseAsset()
        {
            if (!_operationHandle.IsValid())
                return false;

            Addressables.Release(_operationHandle);
            _operationHandle = default;
            return true;
        }

        public void UnloadScene(Action onSceneUnloaded)
        {
            if (!_operationHandle.IsValid())
            {
                onSceneUnloaded?.Invoke();
            }
            else
            {
                Internal_UnloadScene(onSceneUnloaded).Forget();
            }
        }

        private async UniTaskVoid Internal_UnloadScene(Action onSceneUnloaded)
        {
            await UnloadSceneAsync();
            onSceneUnloaded?.Invoke();
        }

        public async UniTask UnloadSceneAsync()
        {
            if (!_operationHandle.IsValid())
                return;

            // Store handle reference, but make the value invalid
            var handle = _operationHandle.Convert<SceneInstance>();
            _operationHandle = default;

            await Addressables.UnloadSceneAsync(handle, true);
        }
    }

    [Serializable]
    public class StaticCrystAssetT<T> : StaticCrystAsset, IWeakAssetT<T> where T : Object
    {
#if UNITY_EDITOR
        [SerializeField]
        private T _packedAsset = null;

        public override Object EditorAsset { get => _packedAsset; set => _packedAsset = value as T; }

        public T EditorAssetT { get => _packedAsset; set => _packedAsset = value; }

        public StaticCrystAssetT(T packedAsset)
        {
            _packedAsset = packedAsset;
            ValidateGUID();
        }
#endif

        public T AssetT => base.Asset as T;

        public override Type AssetType => typeof(T);

        public StaticCrystAssetT(string guid) : base(guid) { }

        public void LoadAsset(Action<T> onLoaded) => base.LoadAsset<T>(onLoaded);

        public async UniTask<T> LoadAssetAsync() => await base.LoadAssetAsync<T>();
    }
}
