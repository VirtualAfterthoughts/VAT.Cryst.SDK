using Cysharp.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

namespace VAT.Scene
{
    public static class CrystSceneManager
    {
        private static SceneLoader _sceneSession = new();

        public static SceneLoader SceneSession => _sceneSession;

        public static void LoadLevel(string levelAddress)
        {
            var level = new LevelContentReference() { Address = levelAddress };

            LoadLevel(level, null);
        }

        public static void LoadLevel(string levelAddress, string loadLevelAddress)
        {
            var level = new LevelContentReference() { Address = levelAddress };
            var loadLevel = new LevelContentReference() { Address = loadLevelAddress };

            LoadLevel(level, loadLevel);
        }

        public static void LoadLevel(LevelContentReference level)
        {
            LoadLevel(level, null);
        }

        public static void LoadLevel(LevelContentReference level, LevelContentReference loadLevel)
        {
            loadLevel.TryGetContent(out var loadLevelContent);

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
