using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Entities.PhysX;

using VAT.Shared.Extensions;

namespace VAT.Audio
{
    [RequireComponent(typeof(CrystRigidbody))]
    public class AccelerationSFX : MonoBehaviour
    {
        [SerializeField]
        private CrystRigidbody _rigidbody = null;

        [SerializeField]
        private AudioClip[] _jerkClips = new AudioClip[0];

        [SerializeField]
        [Min(0f)]
        [Tooltip("The acceleration required to play the sound. Measured in m/s^2.")]
        private float _minAcceleration = 50f;

        [SerializeField]
        [Min(0f)]
        [Tooltip("The acceleration required for maximum volume. Measured in m/s^2.")]
        private float _maxAcceleration = 1000f;

        public void PlaySFX(float magnitude)
        {
            float volume = CalculateVolume(magnitude);
            if (volume <= 0f)
            {
                return;
            }

            float pitch = UnityEngine.Random.Range(0.7f, 1.2f);

            AudioSpawner.Spawn(new AudioSpawner.AudioRequestInfo()
            {
                clip = _jerkClips.GetRandom(),
                position = transform.position,
                settings = new AudioPlaySettings()
                {
                    volume = volume,
                    pitch = pitch,
                },
                playCallback = OnPlay,
            });
        }

        private void OnPlay(AudioSpawner.AudioCallbackInfo info)
        {
            info.audioPlayer.Velocity = _rigidbody.Rigidbody.velocity;
        }

        private Vector3 _lastVelocity;
        private float _timeSinceSFX = 0f;

        private float _lastMagnitude = 0f;

        private float CalculateVolume(float magnitude)
        {
            float volume = (magnitude - _minAcceleration) / (_maxAcceleration - _minAcceleration);

            if (volume <= 0.1f)
            {
                volume = 0f;
            }

            return Mathf.Clamp01(volume);
        }

        public void FixedUpdate()
        {
            if (!_rigidbody.HasBody)
            {
                return;
            }

            var velocity = _rigidbody.Rigidbody.velocity;
            var acceleration = (velocity - _lastVelocity) / Time.fixedDeltaTime;

            float magnitude = acceleration.magnitude;

            _timeSinceSFX += Time.deltaTime;
            float minimumTime = Mathf.Lerp(0.05f, 0.25f, CalculateVolume(_lastMagnitude));

            if (magnitude > _minAcceleration && (_timeSinceSFX > minimumTime || magnitude > _lastMagnitude * 2f))
            {
                PlaySFX(magnitude);
                _timeSinceSFX = 0f;
                _lastMagnitude = magnitude;
            }

            _lastVelocity = velocity;
        }
    }
}
