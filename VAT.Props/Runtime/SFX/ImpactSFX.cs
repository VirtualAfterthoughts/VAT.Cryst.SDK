using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Packaging;
using VAT.Props;
using VAT.Shared.Extensions;

namespace VAT.Audio
{
    public class ImpactSFX : MonoBehaviour
    {
        [SerializeField]
        private ShardReferenceT<ImpactMaterial> _materialReference;

        [SerializeField]
        private float _minVelocity = 0.5f;

        [SerializeField]
        private float _groupSplit = 1f;

        private float _lastCollisionTime = 0f;

        private ImpactMaterial.ImpactLevel _lastGroup;

        private ImpactMaterial.ImpactLevel? GetGroup(float velocity)
        {
            if (!_materialReference.TryGetShard(out var shard))
            {
                return null;
            }

            var (valid, group) = shard.GetImpactGroup();

            if (!valid)
            {
                return null;
            }

            ImpactMaterial.ImpactLevel? foundLevel = null;
            for (var i = 0; i < group.impactLevels.Length; i++)
            {
                float min = _groupSplit * i + _minVelocity;
                if (velocity > min)
                {
                    foundLevel = group.impactLevels[i];
                }
                else
                {
                    break;
                }
            }

            return foundLevel;
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnCollision(collision);
        }

        private void ProcessContact(float volume, ContactPoint point)
        {
            if (!_lastGroup.audioCollection.TryGetShard(out var collection))
            {
                return;
            }

            var clip = collection.GetRandomAudioClip();

            if (clip.TryGetShard(out var shard))
            {
                shard.MainAssetT.LoadAsset((a) =>
                {
                    var normal = point.normal;
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
                        clip = a,
                        position = point.point,
                        settings = new AudioPlaySettings()
                        {
                            volume = volume,
                            pitch = Random.Range(0.7f, 1.5f),
                        },
                        playCallback = (i) => { OnAudioPlay(i, velocity); }
                    });
                });
            }
        }

        private void OnCollision(Collision collision)
        {
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

                _lastGroup = group.Value;

                ProcessContact(volume, collision.GetContact(0));

                _lastCollisionTime = Time.realtimeSinceStartup;
            }
        }

        private void OnAudioPlay(AudioSpawner.AudioCallbackInfo info, Vector3 velocity)
        {
            info.audioPlayer.Velocity = velocity;
        }
    }
}
