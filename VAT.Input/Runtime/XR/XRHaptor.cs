using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;
using UnityEngine.XR.OpenXR.Input;

using VAT.Input.Haptic;

namespace VAT.Input.XR
{
    public class XRHaptor : IInputHaptor
    {
        private readonly InputAction _hapticAction;

        private readonly Handedness _handedness;

        public XRHaptor(InputAction hapticAction, Handedness handededness)
        {
            _hapticAction = hapticAction;
            _handedness = handededness;
        }

        private InputDevice GetController()
        {
            return _handedness switch
            {
                Handedness.LEFT => UnityEngine.InputSystem.XR.XRController.leftHand,
                Handedness.RIGHT => UnityEngine.InputSystem.XR.XRController.rightHand,
                _ => null,
            };
        }

        public void SendHapticImpulse(HapticImpulse impulse)
        {
            OpenXRInput.SendHapticImpulse(_hapticAction, impulse.amplitude, impulse.frequency, impulse.duration, GetController());
        }

        public void StopHaptics()
        {
            OpenXRInput.StopHaptics(_hapticAction, GetController());
        }
    }
}
