using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input
{
    public struct HapticImpulse
    {
        public float amplitude;
        public float frequency;
        public float duration;

        public HapticImpulse(float amplitude, float duration)
        {
            this.amplitude = amplitude;
            this.frequency = 0f;
            this.duration = duration;
        }

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
