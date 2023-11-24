using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using VAT.Cryst;

using VAT.Shared.Extensions;

namespace VAT.Packaging.Editor
{
    public class PackageCreationWizard : EditorWindow
    {
        public string _title = "My Package";
        public string _author = "Author";

        public static void Initialize()
        {
            PackageCreationWizard window = (PackageCreationWizard)EditorWindow.GetWindow(typeof(PackageCreationWizard), true, "Package Creator");
            window.Show();
        }

        private string BuildAddress()
        {
            return Address.BuildAddress(_author, "Package", _title);
        }

        public void OnGUI()
        {
            // Header
            EditorGUILayout.LabelField("Package Settings", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            // Spacing
            GUILayout.Space(5);

            string checkAddress = BuildAddress();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Package Address", checkAddress);
            EditorGUI.EndDisabledGroup();

            _title = EditorGUILayout.TextField("Package Title", _title);
            _author = EditorGUILayout.TextField("Package Author", _author);

            // Spacing
            GUILayout.Space(5);

            // Header
            EditorGUILayout.LabelField("Options", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            // Spacing
            GUILayout.Space(5);

            if (AssetPackager.Instance.HasPackage(checkAddress))
            {
                EditorGUILayout.HelpBox("There's already a package at that address!", MessageType.Error);
            }
            else if (GUILayout.Button("Create Package", GUILayout.Width(130)))
            {
                InternalCreatePackage();
                Close();
            }
        }

        private void InternalCreatePackage()
        {
            Package package = Package.Create(typeof(Package));
            package.PackageInfo = new()
            {
                Title = _title,
                Author = _author
            };

            package.Address = Address.BuildAddress(_author, "Package", _title);

            var assetsFolderPath = $"Assets/{CrystAssetManager.CRYST_ASSETS_FOLDER}";
            var packageFolderPath = $"{assetsFolderPath}/{AssetPackager.CRYST_PACKAGES_FOLDER}";

            if (!AssetDatabase.IsValidFolder(packageFolderPath))
            {
                AssetDatabase.CreateFolder(assetsFolderPath, AssetPackager.CRYST_PACKAGES_FOLDER);
            }

            var addressPath = $"{packageFolderPath}/{package.Address}";
            if (!AssetDatabase.IsValidFolder(addressPath))
            {
                AssetDatabase.CreateFolder(packageFolderPath, package.Address);
            }

            var filePath = $"{addressPath}/{_title}.asset";

            if (AssetDatabase.LoadAllAssetsAtPath(filePath).Length > 0)
                AssetDatabase.DeleteAsset(filePath);

            AssetDatabase.CreateAsset(package, filePath);

            package.ForceSerialize();

            AssetPackager.EditorForceRefresh();

            // Show file in editor
            Selection.SetActiveObjectWithContext(package, package);
        }
    }
}
