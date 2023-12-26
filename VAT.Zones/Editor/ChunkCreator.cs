using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using VAT.Packaging;

namespace VAT.Zones.Editor
{
    public static class ChunkCreator
    {
        private const string _itemName = "GameObject/Crystalline/Chunks/Create Chunk";

        [MenuItem(_itemName, true)]
        public static bool ValidateItem()
        {
            bool isValid = Selection.activeGameObject != null && !Application.isPlaying;
            return isValid;
        }

        [MenuItem(_itemName)]
        public static void CreateNewChunk(MenuCommand command)
        {
            if (command.context != Selection.activeGameObject)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Debug.LogError("Cannot create a new chunk while in play mode!");
                return;
            }

            var gameObjects = Selection.gameObjects;

            var activeScene = SceneManager.GetActiveScene();
            var scenePath = activeScene.path;

            if (string.IsNullOrWhiteSpace(scenePath) || !activeScene.IsValid())
            {
                Debug.LogError("Cannot create new chunk as the active scene is invalid!");
                return;
            }

            var sceneDirectory = Path.GetDirectoryName(scenePath);
            var chunkFolder = $"{activeScene.name} Chunks";
            var chunkDirectory = $@"{sceneDirectory}\{chunkFolder}";

            if (!AssetDatabase.IsValidFolder(chunkDirectory))
            {
                AssetDatabase.CreateFolder(sceneDirectory, chunkFolder);
            }

            int number = 0;
            string chunkName = $"{activeScene.name}_Chunk{number}";
            var chunkPath = $@"{chunkDirectory}\{chunkName}.unity";
            
            while (AssetDatabase.LoadAssetAtPath<SceneAsset>(chunkPath))
            {
                number++;
                chunkName = $"{activeScene.name}_Chunk{number}";
                chunkPath = $@"{chunkDirectory}\{chunkName}.unity";
            }

            chunkPath = EditorUtility.SaveFilePanel("New Chunk", chunkDirectory, chunkName, "unity");
            
            if (string.IsNullOrWhiteSpace(chunkPath))
            {
                return;
            }

            if (chunkPath.StartsWith(Application.dataPath))
            {
                chunkPath = "Assets" + chunkPath[Application.dataPath.Length..];
            }
            else
            {
                Debug.LogError("Path to new chunk was not inside of the project!");
                return;
            }

            var newChunk = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            
            foreach (var go in gameObjects)
            {
                SceneManager.MoveGameObjectToScene(go, newChunk);
            }

            EditorSceneManager.SaveScene(newChunk, chunkPath);

            EditorSceneManager.MarkAllScenesDirty();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(chunkPath);

            // Check if we can add the chunk to the level content
            if (AssetPackager.IsReady)
            {
                foreach (var level in AssetPackager.Instance.GetContents<StaticLevelContent>())
                {
                    if (level.MainScene.EditorScene == null)
                        continue;

                    var path = AssetDatabase.GetAssetOrScenePath(level.MainScene.EditorScene);

                    if (path == activeScene.path)
                    {
                        level.Chunks.Add(new StaticCrystChunk(
                            newChunk.name,
                            new StaticCrystScene(AssetDatabase.GUIDFromAssetPath(chunkPath).ToString())
                            ));

                        EditorUtility.SetDirty(level);
                        break;
                    }
                }
            }
        }
    }
}
