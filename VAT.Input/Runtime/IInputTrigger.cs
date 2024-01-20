using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input
{
    public interface IInputTrigger : IInputButton
    {
        float GetAxis();

        float GetForce();
    }
}
