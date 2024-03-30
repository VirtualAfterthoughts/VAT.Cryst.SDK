using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input
{
    public readonly struct GenericInput : IBasicInput
    {
        private readonly Vector3 _movement;
        private readonly bool _jump;

        public GenericInput(Vector3 movement, bool jump)
        {
            _movement = movement;
            _jump = jump;
        }

        public readonly bool GetJump()
        {
            return _jump;
        }

        public readonly Vector3 GetMovement()
        {
            return _movement;
        }
    }
}
