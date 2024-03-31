using Cysharp.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input.Haptic
{
    public static class HapticHelper
    {
        public static void SendSoftHaptic(IInputHaptor haptor, HapticImpulse impulse)
        {
            SendSoftHapticUniTask(haptor, impulse).Forget();
        }

        private static async UniTaskVoid SendSoftHapticUniTask(IInputHaptor haptor, HapticImpulse impulse)
        {
            float elapsed = 0f;

            while (elapsed < impulse.duration)
            {
                float duration = Time.deltaTime;

                elapsed += duration;

                float amplitude = Mathf.Lerp(0f, impulse.amplitude, elapsed / impulse.duration);
                float frequency = impulse.frequency;

                haptor.SendHapticImpulse(new HapticImpulse(amplitude, frequency, duration));

                await UniTask.Yield();
            }
        }
    }
}
