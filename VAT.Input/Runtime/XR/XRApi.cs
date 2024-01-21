using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Management;

namespace VAT.Input.XR
{
    public class XRApi
    {
        public XRController LeftController { get; }
        public XRController RightController { get; }

        private readonly XRInputActions _inputActions;

        public XRApi() {
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }

            XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
            XRGeneralSettings.Instance.Manager.StartSubsystems();

            _inputActions = new XRInputActions();
            _inputActions.Enable();

            LeftController = new XRController(Handedness.LEFT, _inputActions);
            RightController = new XRController(Handedness.RIGHT, _inputActions);
        }
    }
}
