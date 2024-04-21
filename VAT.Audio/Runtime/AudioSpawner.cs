using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;
using VAT.Pooling;

namespace VAT.Audio
{
    public static class AudioSpawner
    {
        public struct AudioRequestInfo
        {
            public AudioClip clip;

            public Vector3 position;

            public AudioPlaySettings settings;

            public Action<AudioCallbackInfo> playCallback;
        }

        public struct AudioCallbackInfo
        {
            public AssetPoolable assetPoolable;

            public AudioPlayer audioPlayer;
        }

        private static ContentInfo _info = new()
        {
            Title = "Audio Player",
            Hidden = true,
        };

        private static DynamicSpawnableContent _spawnable = null;

        public static void Spawn(AudioRequestInfo info)
        {
            if (_spawnable == null)
            {
                Debug.LogWarning("Tried spawning from the AudioSpawner, but the spawnable isn't ready yet!");
                return;
            }

            var spawnable = new Spawnable(_spawnable.Address)
            {
                rules = new SpawnRules(32, SpawnMode.REUSE_OLDEST),
            };

            AssetSpawner.Register(spawnable);

            var request = new AssetSpawner.SpawnRequestInfo()
            {
                position = info.position,
                spawnable = spawnable,
                spawnCallback = (i) =>
                {
                    OnSpawn(i.assetPoolable, info);
                }
            };

            AssetSpawner.Spawn(request);
        }

        private static void OnSpawn(AssetPoolable poolable, AudioRequestInfo info)
        {
            var player = AudioPlayer.Cache.Get(poolable.gameObject);

            player.Play(info.clip, info.settings);

            info.playCallback?.Invoke(new AudioCallbackInfo()
            {
                assetPoolable = poolable,
                audioPlayer = player,
            });
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void RuntimeInitialize()
        {
            PoolManager.HookOnReady(CreateSpawnable);
        }

        private static void CreateSpawnable()
        {
            var audioPlayerGameObject = new GameObject("Audio Player");
            audioPlayerGameObject.AddComponent<AudioPlayer>();

            audioPlayerGameObject.SetActive(false);
            GameObject.DontDestroyOnLoad(audioPlayerGameObject);

            _spawnable = DynamicContentFactory.Create<DynamicSpawnableContent>(_info, audioPlayerGameObject);
        }
    }
}
