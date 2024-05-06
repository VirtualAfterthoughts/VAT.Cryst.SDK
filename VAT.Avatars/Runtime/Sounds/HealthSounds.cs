using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Audio;

namespace VAT.Avatars.Sounds
{
    [Serializable]
    public struct HealthSounds
    {
        [SerializeField]
        [Tooltip("Sounds for avatar pain, such as being stabbed or shot.")]
        private AudioCollectionReference[] _painLevels;

        [SerializeField]
        [Tooltip("Sounds for avatar healing, such as from consuming food, medkits, etc.")]
        private AudioCollectionReference[] _healLevels;

        [SerializeField]
        [Tooltip("Sounds for avatar death.")]
        private AudioCollectionReference _deathSounds;
    }
}
