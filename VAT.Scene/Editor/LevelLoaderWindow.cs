using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

using VAT.Packaging;

namespace VAT.Scene.Editor
{
    public class LevelLoaderWindow : EditorWindow
    {
        private StaticLevelContent _level, _loadLevel;

        [MenuItem("VAT/Cryst SDK/Tools/Scene/Load Level", priority = -10000)]
        public static void Initialize()
        {
            LevelLoaderWindow window = GetWindow<LevelLoaderWindow>("Level Loader");
            window.Show();
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField("Level Loader", EditorStyles.whiteLargeLabel);

            EditorGUILayout.LabelField("This menu allows you to quickly load a level in Editor or PlayMode.", EditorStyles.boldLabel);

            GUILayout.Space(3);

            _level = EditorGUILayout.ObjectField("Level", _level, typeof(StaticLevelContent), false) as StaticLevelContent;

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            
            _loadLevel = EditorGUILayout.ObjectField("Load Level", _loadLevel, typeof(StaticLevelContent), false) as StaticLevelContent;

            EditorGUI.EndDisabledGroup();

            GUILayout.Space(5);

            if (GUILayout.Button("Load Into Level", GUILayout.Width(100)))
            {
                if (Application.isPlaying)
                {
                    string level = null;
                    string loadLevel = null;

                    if (_level != null)
                        level = _level.Address;

                    if (_loadLevel != null)
                        loadLevel = _loadLevel.Address;

                    CrystSceneManager.LoadLevel(level, loadLevel);
                }
                else
                {
                    var path = AssetDatabase.GetAssetPath(_level.MainAsset.EditorAsset);
                    EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                }
            }
        }
    }
}
