using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Skeleton;

using VAT.Shared.Data;

namespace VAT.Input
{
    public readonly struct GenericHand : IInputHand
    {
        public readonly SimpleTransform Transform => _transform;

        private readonly SimpleTransform _transform;

        private readonly IInputController _controller;

        public GenericHand(SimpleTransform transform, IInputController controller)
        {
            _transform = transform;
            _controller = controller;
        }

        public IInputController GetInputController()
        {
            return _controller;
        }
    }
}
