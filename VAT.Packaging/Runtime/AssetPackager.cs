using Cysharp.Threading.Tasks;

using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.AddressableAssets;

using VAT.Serialization.JSON;
using VAT.Cryst.Game;

using Newtonsoft.Json.Linq;

using System;

namespace VAT.Packaging
{
    public class AssetPackager
    {
        public const string INTERNAL_CRYSTALS_GROUP = "Internal Crystals";
        public const string INTERNAL_CRYSTALS_LABEL = "InternalCrystal";

        public static readonly CrystalLoadOptions InternalLoadOptions = new()
        {
            isInternal = true,
        };

        public static readonly CrystalLoadOptions ExternalLoadOptions = new()
        {
            isInternal = false,
        };

        private static AssetPackager _instance = null;
        public static AssetPackager Instance => _instance;

        private bool _isReady = false;
        private bool _isInitializing = false;

        private static Action _onPackagerReady = null;

        public static bool IsReady => Instance != null && Instance._isReady;

        public bool HasCrystals => CrystalCount > 0;

        public int CrystalCount => IsReady ? Instance._loadedCrystals.Count : 0;

        public int ShardCount => IsReady ? Instance._loadedShards.Count : 0;

        private Dictionary<Address, Crystal> _loadedCrystals;

        private Dictionary<Address, IShard> _loadedShards;

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
            {
                return;
            }

            // Make sure this is the singleton incase it failed to set
            if (_instance != this)
            {
                _instance = this;
            }

            _isInitializing = true;

            _loadedCrystals = new Dictionary<Address, Crystal>();
            _loadedShards = new Dictionary<Address, IShard>();

            // Editor initialize
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                string crystalsPath = CrystAssetManager.GetCrystRelativePath(CRYST_CRYSTALS_FOLDER);
                if (AssetDatabase.IsValidFolder(crystalsPath))
                {
                    string[] folders = Directory.GetDirectories(CrystAssetManager.GetCrystPath(CRYST_CRYSTALS_FOLDER));

                    foreach (var folder in folders)
                    {
                        string[] files = Directory.GetFiles(folder);

                        foreach (var file in files)
                        {
                            if (!file.EndsWith(".asset"))
                                continue;

                            string final = file.Replace(CrystAssetManager.GetProjectPath(), "");

                            var crystal = AssetDatabase.LoadAssetAtPath<Crystal>(final);
                            if (crystal != null)
                                LoadCrystal(crystal);
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
            InternalInitAsync().Forget();
        }

        private async UniTaskVoid InternalInitAsync()
        {
            // Make sure addressables get initialized
            await Addressables.InitializeAsync();

            // Get the resource location of built in crystals
            var keys = await Addressables.LoadResourceLocationsAsync(INTERNAL_CRYSTALS_LABEL).Task;

            if (keys.Count <= 0)
            {
                Debug.Log("No internal crystals were found.");
            }
            else
            {
                // Load built in crystals
                var handle = await Addressables.LoadAssetsAsync<TextAsset>(INTERNAL_CRYSTALS_LABEL, null).Task;

                foreach (var asset in handle)
                {
                    LoadCrystal(asset.text, InternalLoadOptions);
                }
            }

            // Load external crystals (mods)
            // Not implemented

            _isReady = true;

            // Invoke ready
            _onPackagerReady?.Invoke();
            _onPackagerReady = null;
        }

#if UNITY_EDITOR
        public const string CRYST_CRYSTALS_FOLDER = "Crystals";
        public const string CRYST_TEXT_ASSETS_FOLDER = "Text Assets";

        [InitializeOnLoadMethod]
        private static void InternalInitializeEditor()
        {
            // Initialize asset packager
            _instance ??= new AssetPackager(true);
        }

        public static void EditorForceRefresh()
        {
            _instance = null;
            _instance = new AssetPackager(true);
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InternalInitializeRuntime()
        {
            _instance = new AssetPackager();
        }

        public void LoadCrystal(string json)
        {
            LoadCrystal(json, ExternalLoadOptions);
        }

        public void LoadCrystal(Crystal crystal)
        {
            LoadCrystal(crystal, ExternalLoadOptions);
        }

        public void LoadCrystal(string json, CrystalLoadOptions options)
        {
            JSONUnpacker unpacker = new(JObject.Parse(json));
            unpacker.UnpackRoot(out var package, Crystal.Create);

            if (package != null)
            {
                LoadCrystal(package, options);
            }
        }

        public void LoadCrystal(Crystal crystal, CrystalLoadOptions options)
        {
            if (_loadedCrystals.ContainsKey(crystal.Address))
            {
                Debug.LogError("Tried loading a crystal with an already loaded address!", crystal);
                return;
            }

            _loadedCrystals.Add(crystal.Address, crystal);

            foreach (var shard in crystal.Shards)
            {
                LoadShard(shard);
            }

            // Apply the load options
            crystal.Load(options);
        }

        public void LoadShard(IShard shard)
        {
            if (_loadedShards.ContainsKey(shard.Address))
            {
                Debug.LogError($"Tried loading shard {shard.Info.Title} with an already loaded address!");
                return;
            }

            _loadedShards.Add(shard.Address, shard);
        }

        public void UnloadShard(IShard shard)
        {
            bool hasShard = _loadedShards.TryGetValue(shard.Address, out var foundShard);

            if (!hasShard || foundShard != shard)
            {
                Debug.LogError($"Tried unloading shard {shard.Info.Title}, but it was not loaded!");
                return;
            }

            _loadedShards.Remove(shard.Address);
        }

        public bool HasCrystal(Address address)
        {
            return _loadedCrystals.ContainsKey(address);
        }

        public bool TryGetCrystal(Address address, out Crystal crystal)
        {
            if (_loadedCrystals.ContainsKey(address))
            {
                crystal = _loadedCrystals[address];
                return true;
            }

            crystal = default;
            return false;
        }

        public bool HasShard(Address address)
        {
            return _loadedShards.ContainsKey(address);
        }

        public bool TryGetShard(Address address, out IShard shard)
        {
            if (_loadedShards.ContainsKey(address))
            {
                shard = _loadedShards[address];
                return true;
            }

            shard = default;
            return false;
        }

        public bool TryGetShard<T>(Address address, out T shard) where T : IShard
        {
            if (_loadedShards.ContainsKey(address))
            {
                var loaded = _loadedShards[address];

                if (loaded is T result)
                {
                    shard = result;
                    return true;
                }
            }

            shard = default;
            return false;
        }

        public IReadOnlyCollection<Crystal> GetCrystals()
        {
            return _loadedCrystals.Values;
        }

        public IReadOnlyCollection<IShard> GetShards()
        {
            return _loadedShards.Values;
        }

        public IReadOnlyCollection<T> GetShards<T>() where T : IShard
        {
            List<T> shards = new();

            foreach (var shard in _loadedShards.Values)
            {
                if (shard is T value)
                {
                    shards.Add(value);
                }
            }

            return shards;
        }
    }
}
