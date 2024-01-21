using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input
{
    public interface IBasicInput
    {
        Vector3 GetMovement();

        bool GetJump();
    }
}
