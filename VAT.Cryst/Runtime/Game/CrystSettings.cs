using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using VAT.Cryst.Addressables;

namespace VAT.Cryst.Game
{
    using UnityEngine.AddressableAssets;

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

        private static Action _onSettingsLoaded = null;

        [SerializeField]
        private List<SubCrystSettings> _subSettings = new();

        public SubCrystSettings GetSettings(Type type)
        {
            foreach (var settings in _subSettings)
            {
                if (settings.GetType() == type)
                {
                    return settings;
                }
            }

            return null;
        }

        public TSettings GetSettings<TSettings>() where TSettings : SubCrystSettings
        {
            foreach (var settings in _subSettings)
            {
                if (settings is TSettings result)
                {
                    return result;
                }
            }

            return null;
        }

        public void AddSettings(SubCrystSettings settings)
        {
            _subSettings.Add(settings);

#if UNITY_EDITOR
            ValidateSettings();
#endif
        }

        public void RemoveSettings(SubCrystSettings settings)
        {
            _subSettings.Remove(settings);

#if UNITY_EDITOR
            ValidateSettings();
#endif
        }

#if UNITY_EDITOR
        private void ValidateSettings()
        {
            var path = AssetDatabase.GetAssetPath(this);
            
            var allSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

            foreach (var subAsset in allSubAssets)
            {
                if (subAsset is not SubCrystSettings subSettings)
                {
                    continue;
                }

                // Not in list?
                if (!_subSettings.Contains(subSettings))
                {
                    AssetDatabase.RemoveObjectFromAsset(subSettings);
                }
            }
        }
#endif

        public static void HookOnLoad(Action action)
        {
            if (_loadedSettings != null)
            {
                action();
            }
            else
            {
                _onSettingsLoaded += action;
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

            // Invoke load callback
            _onSettingsLoaded?.Invoke();
            _onSettingsLoaded = null;
        }

#if UNITY_EDITOR
        [MenuItem("VAT/Cryst SDK/Settings")]
        private static void OpenSettings()
        {
            if (_loadedSettings != null)
            {
                Selection.SetActiveObjectWithContext(_loadedSettings, _loadedSettings);
                return;
            }

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
            ValidateSettings();
        }

        public void EditorValidateAddressable()
        {
            if (this.IsAddressable())
            {
                return;
            }

            var group = AddressablesExtensions.CreateOrFindGroup("Settings");

            if (!group.GetSchema<PersistentGroupSchema>())
            {
                group.AddSchema<PersistentGroupSchema>();
            }

            var entry = this.SetAddressable(group);

            entry.SetAddress(Address);
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

                return true;
            }

            return false;
        }
#endif
    }
}
