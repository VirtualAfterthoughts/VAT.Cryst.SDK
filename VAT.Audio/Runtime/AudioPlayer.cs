using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Pooling;

using VAT.Shared.Extensions;
using VAT.Shared.Utilities;

namespace VAT.Audio
{
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public sealed class AudioPlayer : MonoBehaviour
    {
        public static ComponentCache<AudioPlayer> Cache = new();

        [SerializeField]
        private AudioSource _source = null;

        private AssetPoolable _poolable = null;

        public AudioSource Source => _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();

            _poolable = gameObject.AddOrGetComponent<AssetPoolable>();

            Cache.Add(gameObject, this);
        }

        private void OnDestroy()
        {
            Cache.Remove(gameObject, this);
        }

        public void Play(AudioClip clip, AudioPlaySettings settings)
        {
            _source.volume = settings.volume;
            _source.pitch = settings.pitch;

            _source.clip = clip;

            _source.spatialBlend = 1f;

            _source.Play();
        }

        public void LateUpdate()
        {
            if (!_source.isPlaying && _poolable.CanDespawn)
            {
                _poolable.Despawn();
            }
        }
    }
}
