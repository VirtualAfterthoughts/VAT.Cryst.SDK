using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using UnityEngine;

using VAT.Cryst.Game;

using VAT.Packaging;

namespace VAT.Audio
{
    [DisplayName("Audio Settings")]
    public class CrystAudioSettings : SubCrystSettings
    {
        [SerializeField]
        private SpawnableShardReference _audioPlayerReference;

        public SpawnableShardReference AudioPlayerReference
        {
            get
            {
                return _audioPlayerReference;
            }
        }
    }
}
