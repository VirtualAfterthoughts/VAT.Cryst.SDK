using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Cryst.Game;
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

        private static SpawnableShardReference _audioPlayerReference = null;

        public static void Spawn(AudioRequestInfo info)
        {
            if (_audioPlayerReference == null)
            {
                Debug.LogWarning("Tried spawning from the AudioSpawner, but the spawnable hasn't been loaded!");
                return;
            }

            var spawnable = new Spawnable(_audioPlayerReference.Address)
            {
                rules = new SpawnRules(32, SpawnMode.REUSE_OLDEST),
            };

            GlobalSpawner.Register(spawnable);

            var request = new GlobalSpawner.SpawnRequestInfo()
            {
                position = info.position,
                spawnable = spawnable,
                spawnCallback = (i) =>
                {
                    OnSpawn(i.assetPoolable, info);
                }
            };

            GlobalSpawner.Spawn(request);
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
            CrystSettings.HookOnLoad(OnLoadSettings);
        }

        private static void OnLoadSettings()
        {
            var settings = CrystSettings.LoadedSettings;
            var audioSettings = settings.GetSettings<CrystAudioSettings>();

            if (audioSettings == null)
            {
                Debug.LogWarning("The current CrystSettings is missing AudioSettings! Audio spawning will not function!");
                return;
            }

            if (audioSettings.AudioPlayerReference == null || audioSettings.AudioPlayerReference.Address == Address.EMPTY)
            {
                Debug.LogWarning("The current CrystAudioSettings contain an invalid audio player reference! Audio spawning will not function!");
                return;
            }

            _audioPlayerReference = audioSettings.AudioPlayerReference;
        }
    }
}
