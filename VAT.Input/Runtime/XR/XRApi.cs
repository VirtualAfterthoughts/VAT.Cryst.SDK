using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

namespace VAT.Input.XR
{
    public class XRApi
    {
        public XRController LeftController { get; }
        public XRController RightController { get; }

        public XRHand LeftHand { get; }
        public XRHand RightHand { get; }

        private readonly XRInputActions _inputActions;

        public XRApi() {
            var manager = XRGeneralSettings.Instance.Manager;
            if (manager.activeLoader != null)
            {
                manager.StopSubsystems();
                manager.DeinitializeLoader();
            }

            manager.InitializeLoaderSync();
            manager.StartSubsystems();

            _inputActions = new XRInputActions();
            _inputActions.Enable();

            LeftController = new XRController(Handedness.LEFT, _inputActions);
            RightController = new XRController(Handedness.RIGHT, _inputActions);

            var xrHandSubsystem = manager.activeLoader.GetLoadedSubsystem<XRHandSubsystem>();
            
            if (xrHandSubsystem != null)
            {
                LeftHand = new XRHand(new XRHandPoseProvider(Handedness.LEFT, xrHandSubsystem));
                RightHand = new XRHand(new XRHandPoseProvider(Handedness.RIGHT, xrHandSubsystem));
            }
        }
    }
}
