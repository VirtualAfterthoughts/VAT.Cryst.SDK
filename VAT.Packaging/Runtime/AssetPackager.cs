using Cysharp.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.AddressableAssets;

using VAT.Serialization.JSON;
using VAT.Cryst;

using Newtonsoft.Json.Linq;

using System.Linq;
using System;

namespace VAT.Packaging
{
    public class AssetPackager
    {
        public const string INTERNAL_PACKAGES_GROUP = "Internal Packages";
        public const string INTERNAL_PACKAGES_LABEL = "InternalPackage";

        public static readonly PackageLoadOptions InternalLoadOptions = new()
        {
            isInternal = true,
        };

        public static readonly PackageLoadOptions ExternalLoadOptions = new()
        {
            isInternal = false,
        };

        private static AssetPackager _instance = null;
        public static AssetPackager Instance => _instance;

        private bool _isReady = false;
        private bool _isInitializing = false;

        private static Action _onPackagerReady = null;

        public static bool IsReady => Instance != null && Instance._isReady;

        public bool HasPackages => PackageCount > 0;

        public int PackageCount => _packageCount;

        public int ContentCount => _contentCount;

        private Dictionary<Address, Package> _loadedPackages;
        private int _packageCount;

        private Dictionary<Address, IContent> _loadedContent;
        private int _contentCount;

        public AssetPackager(bool init = true)
        {
            if (init)
            {
                Init();
            }
        }

        public static void HookOnReady(Action action)
        {
            if (IsReady)
            {
                action.Invoke();
            }
            else
            {
                _onPackagerReady += action;
            }
        }

        public void Init()
        {
            if (_isInitializing)
                return;

            _isInitializing = true;

            _loadedPackages = new Dictionary<Address, Package>();
            _loadedContent = new Dictionary<Address, IContent>();

            // Editor initialize
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                string packagePath = CrystAssetManager.GetProjectRelativePath(CRYST_PACKAGES_FOLDER);
                if (AssetDatabase.IsValidFolder(packagePath))
                {
                    string[] folders = Directory.GetDirectories(CrystAssetManager.GetPath(CRYST_PACKAGES_FOLDER));

                    foreach (var folder in folders)
                    {
                        string[] files = Directory.GetFiles(folder);

                        foreach (var file in files)
                        {
                            if (!file.EndsWith(".asset"))
                                continue;

                            string final = file.Replace(CrystAssetManager.GetProjectPath(), "");

                            var package = AssetDatabase.LoadAssetAtPath<Package>(final);
                            if (package != null)
                                LoadPackage(package);
                        }
                    }
                }

                _isReady = true;

                // Invoke ready
                _onPackagerReady?.Invoke();
                _onPackagerReady = null;

                return;
            }
#endif

            // Play mode/built initialize
            Internal_InitAsync().Forget();
        }

        private async UniTaskVoid Internal_InitAsync()
        {
            // Make sure addressables get initialized
            await Addressables.InitializeAsync();

            // Get the resource location of built in packages
            var keys = await Addressables.LoadResourceLocationsAsync(INTERNAL_PACKAGES_LABEL).Task;

            if (keys.Count <= 0)
            {
                Debug.Log("No internal packages were found.");
            }
            else
            {
                // Load built in packages
                var handle = await Addressables.LoadAssetsAsync<TextAsset>(INTERNAL_PACKAGES_LABEL, null).Task;

                foreach (var asset in handle)
                {
                    LoadPackage(asset.text, InternalLoadOptions);
                }
            }

            // Load external packages (mods)
            // Not implemented

            _isReady = true;

            // Invoke ready
            _onPackagerReady?.Invoke();
            _onPackagerReady = null;
        }

#if UNITY_EDITOR
        public const string CRYST_PACKAGES_FOLDER = "Packages";
        public const string CRYST_TEXT_ASSETS_FOLDER = "Text Assets";

        [InitializeOnLoadMethod]
        private static void Internal_InitializeEditor()
        {
            // Initialize asset packager
            CrystAssetManager.HookOnEditorReady(() =>
            {
                _instance ??= new AssetPackager(true);
            });
        }

        public static void EditorForceRefresh()
        {
            _instance = null;
            _instance = new AssetPackager(true);
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Internal_InitializeRuntime()
        {
            _instance = new AssetPackager();
        }

        public void LoadPackage(string json)
        {
            LoadPackage(json, ExternalLoadOptions);
        }

        public void LoadPackage(Package package)
        {
            LoadPackage(package, ExternalLoadOptions);
        }

        public void LoadPackage(string json, PackageLoadOptions options)
        {
            JSONUnpacker unpacker = new(JObject.Parse(json));
            unpacker.UnpackRoot(out var package, Package.Create);

            if (package != null)
            {
                LoadPackage(package, options);
            }
        }

        public void LoadPackage(Package package, PackageLoadOptions options)
        {
            if (_loadedPackages.ContainsKey(package.Address))
            {
                Debug.LogError("Tried loading a package with an already loaded address!", package);
                return;
            }

            _loadedPackages.Add(package.Address, package);

            foreach (var content in package.Contents)
            {
                LoadContent(content);
            }

            // Apply the load options
            package.Load(options);

            // Update information
            _packageCount++;
        }

        public void LoadContent(IContent content)
        {
            if (_loadedContent.ContainsKey(content.Address))
            {
                Debug.LogError($"Tried loading content {content.Info.Title} with an already loaded address!");
                return;
            }

            _loadedContent.Add(content.Address, content);

            _contentCount++;
        }

        public bool HasPackage(Address address)
        {
            return _loadedPackages.ContainsKey(address);
        }

        public bool TryGetPackage(Address address, out Package package)
        {
            if (_loadedPackages.ContainsKey(address))
            {
                package = _loadedPackages[address];
                return true;
            }

            package = default;
            return false;
        }

        public bool HasContent(Address address)
        {
            return _loadedContent.ContainsKey(address);
        }

        public bool TryGetContent(Address address, out IContent content)
        {
            if (_loadedContent.ContainsKey(address))
            {
                content = _loadedContent[address];
                return true;
            }

            content = default;
            return false;
        }

        public bool TryGetContent<T>(Address address, out T content) where T : IContent
        {
            if (_loadedContent.ContainsKey(address))
            {
                var loaded = _loadedContent[address];

                if (loaded is T result)
                {
                    content = result;
                    return true;
                }
            }

            content = default;
            return false;
        }

        public IReadOnlyCollection<Package> GetPackages()
        {
            return _loadedPackages.Values;
        }

        public IReadOnlyCollection<IContent> GetContents()
        {
            return _loadedContent.Values;
        }
    }
}
