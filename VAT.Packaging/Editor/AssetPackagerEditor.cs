using System;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace VAT.Packaging.Editor {
    public class AssetPackagerEditor : EditorWindow {
        private const int BUTTON_SPACE = 3;

        private static TreeViewState _treeViewState;

        private AssetPackagerTreeView _packageTreeView;

        private string _searchQuery;

        [MenuItem("VAT/Cryst SDK/Asset Packager", priority = -10000)]
        public static void Initialize() 
        {
            if (!Application.isPlaying)
            {
                AssetPackager.EditorForceRefresh();
            }

            AssetPackagerEditor window = GetWindow<AssetPackagerEditor>("Asset Packager");
            window.Show();
        }

        private void OnEnable() 
        {
            _treeViewState ??= new TreeViewState();

            _packageTreeView = new AssetPackagerTreeView(_treeViewState);
        }

        public void OnGUI() 
        {
            if (AssetPackager.IsReady) 
            {
                if (AssetPackager.Instance.HasPackages)
                {
                    GUILayout.Label("AssetPackager is ready!");
                }
                else
                {
                    GUILayout.Label("Welcome to the AssetPackager! Create a Package using the button below.");
                }

                DrawSpace();

                EditorGUILayout.BeginHorizontal();

                DrawCreatePackageButton();

                EditorGUILayout.EndHorizontal();

                DrawSpace();

                EditorGUILayout.BeginHorizontal();

                DrawRefreshButton();

                EditorGUILayout.EndHorizontal();

                DrawSpace();

                _searchQuery = EditorGUILayout.TextField(_searchQuery, EditorStyles.toolbarSearchField);
                var rect = GUILayoutUtility.GetAspectRect(2);

                _packageTreeView.searchString = _searchQuery;
                _packageTreeView.OnGUI(rect);
            }
            else 
            {
                GUILayout.Label("AssetPackager is not ready.");

                DrawRefreshButton();
            }
        }

        private void DrawSpace()
        {
            GUILayout.Space(BUTTON_SPACE);
        }

        private void DrawCreatePackageButton()
        {
            if (GUILayout.Button("Create Package", GUILayout.Height(20), GUILayout.Width(120)))
            {
                PackageCreationWizard.Initialize();
                Close();
            }
        }

        private void DrawRefreshButton()
        {
            if (GUILayout.Button("Refresh", GUILayout.Width(100)))
            {
                AssetPackager.EditorForceRefresh();
                OnEnable();
            }
        }
    }
}