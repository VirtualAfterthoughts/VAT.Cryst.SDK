#if ENABLE_INPUT_SYSTEM

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace VAT.Cryst.Extensions
{
    public static class InputActionExtensions
    {
        public static void AddCallback(this InputAction action, Action<InputAction.CallbackContext> callback)
        {
            action.started += callback;
            action.performed += callback;
            action.canceled += callback;
        }

        public static void RemoveCallback(this InputAction action, Action<InputAction.CallbackContext> callback)
        {
            action.started -= callback;
            action.performed -= callback;
            action.canceled -= callback;
        }
    }
}

#endif