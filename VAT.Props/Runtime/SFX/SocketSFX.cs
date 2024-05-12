using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Audio;

using VAT.Interaction.Attachments;
using VAT.Packaging;

namespace VAT.Props
{
    public class SocketSFX : MonoBehaviour
    {
        [SerializeField]
        private Socket _socket = null;

        [SerializeField]
        private AudioCollectionReference _insertSounds;

        [SerializeField]
        private AudioCollectionReference _ejectSounds;

        private void OnEnable()
        {
            _socket.OnLockPlug += OnLockPlug;
            _socket.OnUnlockPlug += OnUnlockPlug;
        }

        private void OnDisable()
        {
            _socket.OnLockPlug -= OnLockPlug;
            _socket.OnUnlockPlug -= OnUnlockPlug;
        }

        private void OnLockPlug(Plug plug)
        {
            PlayFromCollection(plug, _insertSounds);
        }

        private void PlayFromCollection(Plug plug, AudioCollectionReference collection)
        {
            if (!collection.TryGetShard(out var shard))
            {
                return;
            }

            if (!shard.GetRandomAudioClip().TryGetShard(out var clip))
            {
                return;
            }

            PlayClip(plug, clip);
        }

        private void PlayClip(Plug plug, IAudioClipShard clip)
        {
            clip.MainAssetT.LoadAsset((c) =>
            {
                AudioSpawner.Spawn(new AudioSpawner.AudioRequestInfo()
                {
                    clip = c,
                    position = plug.transform.position,
                    settings = new AudioPlaySettings()
                    {
                        volume = 1f,
                        pitch = Random.Range(0.9f, 1.1f),
                    }
                });
            });
        }

        private void OnUnlockPlug(Plug plug)
        {
            PlayFromCollection(plug, _ejectSounds);
        }
    }
}
