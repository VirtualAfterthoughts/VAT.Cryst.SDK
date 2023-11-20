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
                Internal_LoadLevelAsync(levelContent, loadLevelContent).Forget();
            }
        }

        private static async UniTaskVoid Internal_LoadLevelAsync(ILevelContent level, ILevelContent loadLevel)
        {
            _sceneSession = new SceneLoader(level, loadLevel);
            await _sceneSession.Load();
        }
    }
}
