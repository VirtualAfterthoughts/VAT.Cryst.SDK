using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Skeleton;

using VAT.Shared.Data;

namespace VAT.Input
{
    public readonly struct GenericHand : IHand
    {
        public readonly SimpleTransform Transform => _transform;

        private readonly SimpleTransform _transform;

        private readonly IInputController _controller;

        private readonly IInputHand _hand;

        private readonly HandActions _actions;

        public GenericHand(SimpleTransform transform, IInputController controller, IInputHand hand, HandActions actions)
        {
            _transform = transform;
            _controller = controller;
            _hand = hand;
            _actions = actions;
        }

        public IInputController GetInputControllerOrNull()
        {
            return _controller;
        }

        public IInputHand GetInputHandOrNull()
        {
            return _hand;
        }
        public HandActions GetActionsOrNull()
        {
            return _actions;
        }
    }
}
