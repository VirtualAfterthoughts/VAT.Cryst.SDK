using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using VAT.Cryst.Game;

using VAT.Shared.Extensions;

namespace VAT.Packaging.Editor
{
    public class CrystalCreationWizard : EditorWindow
    {
        public string _title = "My Crystal";
        public string _author = "Author";

        public static void Initialize()
        {
            CrystalCreationWizard window = (CrystalCreationWizard)EditorWindow.GetWindow(typeof(CrystalCreationWizard), true, "Crystal Creator");
            window.Show();
        }

        private string BuildAddress()
        {
            return Address.BuildAddress(_author, "Crystal", _title);
        }

        public void OnGUI()
        {
            // Header
            EditorGUILayout.LabelField("Crystal Settings", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            // Spacing
            GUILayout.Space(5);

            string checkAddress = BuildAddress();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Crystal Address", checkAddress);
            EditorGUI.EndDisabledGroup();

            _title = EditorGUILayout.TextField("Crystal Title", _title);
            _author = EditorGUILayout.TextField("Crystal Author", _author);

            // Spacing
            GUILayout.Space(5);

            // Header
            EditorGUILayout.LabelField("Options", EditorStyles.whiteLargeLabel, GUILayout.Height(20));

            // Spacing
            GUILayout.Space(5);

            if (AssetPackager.Instance.HasCrystal(new(checkAddress)))
            {
                EditorGUILayout.HelpBox("There's already a crystal at that address!", MessageType.Error);
            }
            else if (GUILayout.Button("Create Crystal", GUILayout.Width(130)))
            {
                CreateCrystal();
                Close();
            }
        }

        private void CreateCrystal()
        {
            Crystal crystal = Crystal.Create(typeof(Crystal));
            crystal.CrystalInfo = new()
            {
                Title = _title,
                Author = _author
            };

            crystal.Address = new(Address.BuildAddress(_author, "Crystal", _title));

            var crystalFolderPath = CrystAssetManager.GetCrystRelativePath(AssetPackager.CRYST_CRYSTALS_FOLDER);

            CrystAssetManager.EnsureCrystFolderExists(crystalFolderPath);

            var addressPath = $"{crystalFolderPath}/{crystal.Address}";
            if (!AssetDatabase.IsValidFolder(addressPath))
            {
                AssetDatabase.CreateFolder(crystalFolderPath, crystal.Address.ID);
            }

            var filePath = $"{addressPath}/_{_title}.asset";

            if (AssetDatabase.LoadAllAssetsAtPath(filePath).Length > 0)
            {
                AssetDatabase.DeleteAsset(filePath);
            }

            AssetDatabase.CreateAsset(crystal, filePath);

            EditorUtility.SetDirty(crystal);

            AssetPackager.EditorForceRefresh();

            // Show file in editor
            Selection.SetActiveObjectWithContext(crystal, crystal);
        }
    }
}
