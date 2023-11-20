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
    public interface IWeakAsset
    {
#if UNITY_EDITOR
        Object EditorAsset { get; }
#endif

        Object Asset { get; }

        Type AssetType { get; }

        AssetLoadStatus Status { get; }

        void LoadAsset<T>(Action<T> onLoaded) where T : Object;

        void LoadScene(Action<SceneInstance> onLoaded, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true);

        UniTask<SceneInstance> LoadSceneAsync(LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true);

        void UnloadScene(Action onSceneUnloaded);

        UniTask UnloadSceneAsync();

        bool ReleaseAsset();
    }

    public interface IWeakAssetT<T> : IWeakAsset where T : Object
    {
#if UNITY_EDITOR
        T EditorAssetT { get; }
#endif

        T AssetT { get; }

        void LoadAsset(Action<T> onLoaded);
    }
}
