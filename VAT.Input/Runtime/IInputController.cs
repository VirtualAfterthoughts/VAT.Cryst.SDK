using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Input
{
    public interface IInputController
    {
        float GetGripForce()
        {
            float force = 0f;

            var trigger = GetTriggerOrNull();
            if (trigger != null)
            {
                force += trigger.GetForce() * 0.25f;
            }

            var grip = GetGripOrNull();
            if (grip != null)
            {
                force += grip.GetForce() * 0.75f;
            }

            return force;
        }

        bool HasForceSensor();

        IInputTrigger GetTriggerOrNull();

        IInputTrigger GetGripOrNull();

        IInputTrackpad GetThumbstickOrNull();

        IInputTrackpad GetTrackpadOrNull();

        IInputButton GetPrimaryButtonOrNull();

        IInputButton GetSecondaryButtonOrNull();
    }
}
