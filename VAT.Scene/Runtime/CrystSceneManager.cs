using Cysharp.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

namespace VAT.Scene
{
    public struct SceneLoadOptions
    {
        public static readonly SceneLoadOptions Default = new()
        {
            loadLevel = new LevelContentReference(),
        };

        public LevelContentReference loadLevel;
    }

    public static class CrystSceneManager
    {
        private static SceneLoader _sceneSession = new();

        public static SceneLoader SceneSession => _sceneSession;

        public static void LoadLevel(LevelContentReference level)
        {
            LoadLevel(level, SceneLoadOptions.Default);
        }

        public static void LoadLevel(LevelContentReference level, SceneLoadOptions options)
        {
            options.loadLevel.TryGetContent(out var loadLevelContent);

            if (level.TryGetContent(out var levelContent))
            {
                InternalLoadLevelAsync(levelContent, loadLevelContent).Forget();
            }
        }

        private static async UniTaskVoid InternalLoadLevelAsync(ILevelContent level, ILevelContent loadLevel)
        {
            // Unload the active session
            if (_sceneSession != null && _sceneSession.Status == AssetLoadStatus.DONE)
            {
                await _sceneSession.Unload();
            }

            // Create a new session and load the scene
            _sceneSession = new SceneLoader(level, loadLevel);
            await _sceneSession.Load();
        }
    }
}
