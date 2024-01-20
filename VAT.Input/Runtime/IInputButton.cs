using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Input
{
    public interface IInputButton
    {
        bool GetPressed();

        bool GetTouched();
    }
}
