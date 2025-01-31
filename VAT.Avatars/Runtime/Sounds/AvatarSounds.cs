using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Sounds
{
    using VAT.Audio;
    using VAT.Avatars.Integumentary;

    [RequireComponent(typeof(Avatar))]
    public class AvatarSounds : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Sounds for avatar effort, such as a jump.")]
        private AudioCollectionReference[] _effortLevels = new AudioCollectionReference[0];

        [SerializeField]
        [Tooltip("Sounds for avatar footsteps on a generic surface, such as concrete.")]
        private AudioCollectionReference[] _footstepLevels = new AudioCollectionReference[0];

        [SerializeField]
        [Tooltip("Sounds for changes in avatar health.")]
        private HealthSounds _healthSounds = new();

        public AudioCollectionReference[] EffortLevels { get { return _effortLevels; } set { _effortLevels = value; } }

        public AudioCollectionReference[] FootstepLevels { get { return _footstepLevels; } set { _footstepLevels = value; } }

        public HealthSounds HealthSounds { get { return _healthSounds; } set { _healthSounds = value; } }
    }
}
