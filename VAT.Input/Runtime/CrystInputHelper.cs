using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.InputSystem;

namespace VAT.Input
{
    public static class CrystInputHelper
    {
        public static CrystInputActionT<T> CreateCrystInputAction<T>(this InputAction inputAction)
        {
            CrystInputActionT<T> value = new();

            inputAction.started += (ctx) =>
            {
                value.SetValue(ctx.ReadValueAsObject());
                value.SetPhase(CrystInputPhase.STARTED);
            };

            inputAction.performed += (ctx) =>
            {
                value.SetValue(ctx.ReadValueAsObject());
                value.SetPhase(CrystInputPhase.PERFORMED);
            };

            inputAction.canceled += (ctx) =>
            {
                value.SetValue(ctx.ReadValueAsObject());
                value.SetPhase(CrystInputPhase.CANCELED);
            };

            return value;
        }
    }
}
