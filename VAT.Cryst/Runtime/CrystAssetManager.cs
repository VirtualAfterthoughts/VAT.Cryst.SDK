using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VAT.Cryst {
    public static class CrystAssetManager 
    {
        public const string CRYST_ASSETS_FOLDER = "_CrystAssets";

#if UNITY_EDITOR
        public const string PARENT_FOLDER = "Assets";

        public const string PROJECT_RELATIVE_FOLDER = PARENT_FOLDER + "/" + CRYST_ASSETS_FOLDER;

        private static Action _onAssetsReady = null;

        public static bool IsEditorReady => AssetDatabase.IsValidFolder(PROJECT_RELATIVE_FOLDER);

        /// <summary>
        /// Gets the path of the UnityEditor project.
        /// </summary>
        /// <returns></returns>
        public static string GetProjectPath() 
        {
            string path = Path.GetFullPath(Path.Combine(Application.dataPath, "../"));
            return path;
        }

        /// <summary>
        /// Gets the project relative path to a path in CrystAssets.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string GetProjectRelativePath(string path) 
        {
            return Path.Combine(PROJECT_RELATIVE_FOLDER, path);
        }

        /// <summary>
        /// Gets the system path to a path in CrystAssets.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string GetPath(string path)
        {
            return Path.GetFullPath(Path.Combine(GetProjectPath(), GetProjectRelativePath(path)));
        }

        [InitializeOnLoadMethod]
        private static void InternalInitializeEditor() 
        {
            // Create folders
            if (!IsEditorReady) 
            {
                AssetDatabase.CreateFolder(PARENT_FOLDER, CRYST_ASSETS_FOLDER);

                _onAssetsReady?.Invoke();
                _onAssetsReady = null;
            }
        }

        public static void HookOnEditorReady(Action action)
        {
            if (IsEditorReady) 
            {
                action();
            }
            else 
            {
                _onAssetsReady += action;
            }
        }
#endif
    }
}
