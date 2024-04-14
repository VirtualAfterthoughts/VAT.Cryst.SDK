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
        private float _maxAcceleration = 100f;

        [SerializeField]
        [Range(1f, 10f)]
        [Tooltip("The exponent applied to the volume. A value of 1 is linear volume.")]
        private float _pow = 2f;

        public void PlaySFX(float magnitude)
        {
            AudioSpawner.Spawn(new AudioSpawner.AudioRequestInfo()
            {
                clip = _jerkClips.GetRandom(),
                position = transform.position,
                settings = new AudioPlaySettings()
                {
                    volume = Mathf.Pow((magnitude - _minAcceleration) / (_maxAcceleration - _minAcceleration), _pow),
                    pitch = UnityEngine.Random.Range(0.7f, 1.2f)
                },
            });
        }

        private Vector3 _lastVelocity;
        private float _timeSinceSFX = 0f;

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

            if (magnitude > _minAcceleration && _timeSinceSFX > 0.2f)
            {
                PlaySFX(magnitude);
                _timeSinceSFX = 0f;
            }

            _lastVelocity = velocity;
        }
    }
}
