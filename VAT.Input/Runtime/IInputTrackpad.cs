using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Input
{
    public interface IInputTrackpad : IInputButton
    {
        Vector2 GetAxis();
    }
}
