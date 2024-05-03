using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.AddressableAssets;

using VAT.Shared.Extensions;

namespace VAT.Cryst.Game
{
    public class CrystSettings : ScriptableObject
    {
        public const string Address = "CrystSettings";

        private static CrystSettings _loadedSettings = null;
        public static CrystSettings LoadedSettings
        {
            get
            {
                if (_loadedSettings == null)
                {
                    LoadSettingsFromAddress().Forget();
                }

                return _loadedSettings;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeRuntime()
        {
            LoadSettingsFromAddress().Forget();
        }

        private static async UniTask LoadSettingsFromAddress()
        {
            var keys = await Addressables.LoadResourceLocationsAsync(Address).Task;

            if (keys.Count <= 0)
            {
                if (Application.isPlaying)
                {
                    Debug.LogWarning("Missing CrystSettings asset! Make sure the game project is properly configured!");
                }

                return;
            }

            var handle = await Addressables.LoadAssetAsync<CrystSettings>(keys[0]);

            _loadedSettings = handle;
        }

#if UNITY_EDITOR
        [MenuItem("VAT/Cryst SDK/Settings")]
        private static void OpenSettings()
        {
            LoadSettingsFromAddress().ContinueWith(() =>
            {
                // Check loaded settings
                bool success = true;

                if (_loadedSettings == null)
                {
                    success = CreateDefaultSettings();
                }

                if (success)
                {
                    Selection.SetActiveObjectWithContext(_loadedSettings, _loadedSettings);
                }
            });
        }

        public void OnValidate()
        {
            EditorValidateAddressable();
        }

        public void EditorValidateAddressable()
        {
            this.MarkAsAddressable("Settings", Address, Address, true);
        }

        public static bool CreateDefaultSettings()
        {
            if (_loadedSettings == null)
            {
                bool create = EditorUtility.DisplayDialog("Missing Settings", "There are currently no CrystSettings for this project. Add them?", "Yes", "No");

                if (!create)
                {
                    return false;
                }

                var newSettings = CreateInstance<CrystSettings>();

                var path = CrystAssetManager.GetCrystRelativePath("Settings");
                CrystAssetManager.EnsureCrystFolderExists(path);

                var assetPath = path + "/CrystSettings.asset";

                AssetDatabase.CreateAsset(newSettings, assetPath);

                _loadedSettings = AssetDatabase.LoadAssetAtPath<CrystSettings>(assetPath);
                _loadedSettings.EditorValidateAddressable();
            }

            return false;
        }
#endif
    }
}
