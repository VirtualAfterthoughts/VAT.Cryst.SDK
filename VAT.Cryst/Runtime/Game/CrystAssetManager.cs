using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VAT.Cryst.Game
{
    public static class CrystAssetManager
    {
        public const string CRYST_ASSETS_FOLDER = "_CrystAssets";

#if UNITY_EDITOR
        public const string PARENT_FOLDER = "Assets";

        public const string PROJECT_RELATIVE_FOLDER = PARENT_FOLDER + "/" + CRYST_ASSETS_FOLDER;

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
        /// Gets the path relative to the UnityEditor project.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetProjectRelativePath(string path)
        {
            var projectPath = GetProjectPath();
            if (path.Length > projectPath.Length)
            {
                return path[projectPath.Length..];
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the project relative path to a path in CrystAssets.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string GetCrystRelativePath(string path)
        {
            return Path.Combine(PROJECT_RELATIVE_FOLDER, path);
        }

        /// <summary>
        /// Gets the system path to a path in CrystAssets.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string GetCrystPath(string path)
        {
            return Path.GetFullPath(Path.Combine(GetProjectPath(), GetCrystRelativePath(path)));
        }

        /// <summary>
        /// Checks if a cryst relative path folder exists, and if not, creates it.
        /// </summary>
        /// <param name="path"></param>
        public static void EnsureCrystFolderExists(string path)
        {
            var directories = path.Split('/', '\\');

            string lastDirectory = null;

            foreach (var directory in directories)
            {
                if (lastDirectory == null)
                {
                    lastDirectory = directory;
                    continue;
                }

                string totalPath = lastDirectory + "/" + directory;

                if (!AssetDatabase.IsValidFolder(totalPath))
                {
                    AssetDatabase.CreateFolder(lastDirectory, directory);
                }

                lastDirectory = totalPath;
            }
        }
#endif
    }
}
