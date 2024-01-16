using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input
{
    public interface IBasicInput
    {
        public Vector2 GetMovement();

        public float GetCrouch();

        public float GetTurn();

        public bool GetJump();
    }
}
