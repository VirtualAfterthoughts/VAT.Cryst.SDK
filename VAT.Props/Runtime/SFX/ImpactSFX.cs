using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Extensions;

namespace VAT.Audio
{
    public class ImpactSFX : MonoBehaviour
    {
        [SerializeField]
        private VelocityAudioGroup[] _audioGroups = new VelocityAudioGroup[0];

        [SerializeField]
        private float _minVelocity = 0.5f;

        [SerializeField]
        private float _groupSplit = 1f;

        private float _lastCollisionTime = 0f;

        private ContactPoint _lastContactPoint = default;
        private Vector3 _lastVelocity = Vector3.zero;
        private VelocityAudioGroup _lastGroup;
        private float _lastVolume;

        private VelocityAudioGroup? GetGroup(float velocity)
        {
            VelocityAudioGroup? foundGroup = null;
            for (var i = 0; i < _audioGroups.Length; i++)
            {
                float min = _groupSplit * i + _minVelocity;
                if (velocity > min)
                {
                    foundGroup = _audioGroups[i];
                }
                else
                {
                    break;
                }
            }

            return foundGroup;
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnCollision(collision);
        }

        private void ProcessContact(float volume, ContactPoint point)
        {
            var normal = _lastContactPoint.normal;
            var velocity = Vector3.zero;

            if (GetComponent<Rigidbody>() != null)
            {
                velocity = GetComponent<Rigidbody>().GetPointVelocity(point.point);
                velocity = Quaternion.FromToRotation(Vector3.up, normal) * velocity;
                velocity.y = 0f;
                velocity = Quaternion.FromToRotation(normal, Vector3.up) * velocity;
            }

            AudioSpawner.Spawn(new AudioSpawner.AudioRequestInfo()
            {
                clip = _lastGroup.audioClips.GetRandom(),
                position = point.point,
                settings = new AudioPlaySettings()
                {
                    volume = volume,
                    pitch = Random.Range(0.7f, 1.5f),
                },
                playCallback = (i) => { OnAudioPlay(i, velocity); }
            });
        }

        private void OnCollision(Collision collision)
        {
            if (collision.contactCount > 0)
            {
                _lastContactPoint = collision.GetContact(0);
            }
            else
            {
                return;
            }

            float vel = collision.relativeVelocity.magnitude;
            var group = GetGroup(vel);

            if (group.HasValue)
            {
                float min = _minVelocity;
                float volume = (vel - min) / _groupSplit;

                if (volume < 0.2f)
                {
                    return;
                }

                if (Time.realtimeSinceStartup - _lastCollisionTime < 0.1f)
                {
                    return;
                }

                volume /= collision.contactCount;

                _lastGroup = group.Value;

                for (var i = 0; i < collision.contactCount; i++)
                {
                    ProcessContact(volume, collision.GetContact(i));
                }

                _lastCollisionTime = Time.realtimeSinceStartup;
            }
        }

        private void OnAudioPlay(AudioSpawner.AudioCallbackInfo info, Vector3 velocity)
        {
            info.audioPlayer.Velocity = velocity;
        }
    }
}
