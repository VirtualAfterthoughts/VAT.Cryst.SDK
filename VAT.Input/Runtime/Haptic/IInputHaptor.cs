using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input.Haptic
{
    [Serializable]
    public struct HapticImpulse
    {
        /// <summary>
        /// A value from 0 - 1 representing the intensity of the haptic.
        /// </summary>
        [Range(0f, 1f)]
        public float amplitude;

        /// <summary>
        /// Frequency of the impulse in hertz (Hz).
        /// </summary>
        [Min(0f)]
        public float frequency;

        /// <summary>
        /// The time in seconds of the haptic.
        /// </summary>
        [Min(0f)]
        public float duration;

        /// <summary>
        /// Creates a HapticImpulse with a given amplitude and duration.
        /// </summary>
        /// <param name="amplitude">A value from 0 - 1 representing the intensity of the haptic.</param>
        /// <param name="duration">The time in seconds of the haptic.</param>
        public HapticImpulse(float amplitude, float duration)
        {
            this.amplitude = amplitude;
            this.frequency = 0f;
            this.duration = duration;
        }

        /// <summary>
        /// Creates a HapticImpulse with a given amplitude, frequency, and duration.
        /// </summary>
        /// <param name="amplitude">A value from 0 - 1 representing the intensity of the haptic.</param>
        /// <param name="frequency">Frequency of the impulse in hertz (Hz).</param>
        /// <param name="duration">The time in seconds of the haptic.</param>
        public HapticImpulse(float amplitude, float frequency, float duration)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.duration = duration;
        }
    }

    public interface IInputHaptor
    {
        void SendHapticImpulse(HapticImpulse impulse);

        void StopHaptics();
    }
}
