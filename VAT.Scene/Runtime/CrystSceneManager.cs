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
            loadLevel = new LevelShardReference(),
        };

        public LevelShardReference loadLevel;
    }

    public static class CrystSceneManager
    {
        private static SceneLoader _sceneSession = new();

        public static SceneLoader SceneSession => _sceneSession;

        public static void LoadLevel(LevelShardReference level)
        {
            LoadLevel(level, SceneLoadOptions.Default);
        }

        public static void LoadLevel(LevelShardReference level, SceneLoadOptions options)
        {
            options.loadLevel.TryGetShard(out var loadLevelShard);

            if (level.TryGetShard(out var levelContent))
            {
                InternalLoadLevelAsync(levelContent, loadLevelShard).Forget();
            }
        }

        private static async UniTaskVoid InternalLoadLevelAsync(ILevelShard level, ILevelShard loadLevel)
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
