using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VAT.Packaging;

namespace VAT.Scene
{
    public class SceneLoader
    {
        public enum SceneType
        {
            UNKNOWN = 1 << 0,
            ADDRESSABLE = 1 << 1,
            BUILT = 1 << 2,
        }

        private readonly ILevelContent _level, _loadLevel;
        private readonly SceneType _type;
        private AssetLoadStatus _status;

        private SceneInstance _mainSceneInstance;
        private UniTask<SceneInstance> _sceneTask;

        private bool _isValid;

        public ILevelContent Level => _level;

        public AssetLoadStatus Status => _status;

        public SceneType Type => _type;

        public SceneInstance MainSceneInstance => _mainSceneInstance;

        public bool IsValid => _isValid;

        public SceneLoader()
        {
            this._type = SceneType.UNKNOWN;
            this._status = AssetLoadStatus.IDLE;
            this._isValid = false;
        }

        public SceneLoader(ILevelContent level, ILevelContent loadLevel)
        {
            this._level = level;
            this._loadLevel = loadLevel;
            this._status = AssetLoadStatus.IDLE;
            this._type = SceneType.ADDRESSABLE;
        }

        public async UniTask Load()
        {
            // Startup the status
            _status = AssetLoadStatus.LOADING;

            // If we have a loading level, load into it first
            await Internal_EnterLoadLevel();

            // Make sure the main scene is unloaded first
            await _level.MainAsset.UnloadSceneAsync();

            // Now load the main scene
            _sceneTask = _level.MainAsset.LoadSceneAsync(LoadSceneMode.Single, false);

            _mainSceneInstance = await _sceneTask;

            await _mainSceneInstance.ActivateAsync();

            _status = AssetLoadStatus.DONE;
        }

        private async UniTask Internal_EnterLoadLevel()
        {
            // Make sure we have a loading screen first
            if (_loadLevel != null && _loadLevel.MainAsset is StaticCrystScene loadScene)
            {
                await loadScene.LoadSceneAsync(LoadSceneMode.Single, true);
            }
        }
    }
}
