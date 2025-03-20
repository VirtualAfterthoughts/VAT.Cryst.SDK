using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Characters
{
    public enum TurnType
    {
        NONE = 0,
        SNAP = 1,
        SMOOTH = 2,
    }

    [Serializable]
    public class TurnBehaviour : ISubBehaviour
    {
        [Header("Generic")]
        [SerializeField]
        private TurnType _type = TurnType.SMOOTH;

        [Header("Snap Turn")]
        [SerializeField]
        [Range(0f, 180f)]
        private int _snapTurnAngle = 45;

        [Header("Smooth Turn")]
        [SerializeField]
        private float _smoothTurnSpeed = 8f;

        private IBehaviourRig _behaviourRig = null;

        public void OnDeregister(IBehaviourRig behaviourRig)
        {
            _behaviourRig = null;
        }

        public void OnRegister(IBehaviourRig behaviourRig)
        {
            _behaviourRig = behaviourRig;
        }

        public void Solve()
        {
            if (_type == TurnType.NONE)
            {
                return;
            }

            var hand = _behaviourRig.GetPrimaryHand();
            var controller = hand.GetInputController();

            if (controller == null)
            {
                return;
            }

            var thumbstick = controller.GetThumbstick();

            if (thumbstick == null)
            {
                return;
            }

            float turnAxis = thumbstick.GetAxis().x;

            switch (_type)
            {
                case TurnType.SNAP:
                    SnapTurnSolve(turnAxis);
                    break;
                case TurnType.SMOOTH:
                    SmoothTurnSolve(turnAxis);
                    break;
            }
        }

        private bool _snapTurned = false;

        private void SnapTurnSolve(float axis)
        {
            bool shouldSnapTurn = Mathf.Abs(axis) > 0.5f;

            if (shouldSnapTurn && !_snapTurned)
            {
                var root = _behaviourRig.GetBehaviourSpace();
                var head = _behaviourRig.GetLocalHead();

                var headInRoot = root.InverseTransform(head);

                root.Rotation = Quaternion.AngleAxis(_snapTurnAngle * Mathf.Sign(axis), root.Up) * root.Rotation;

                root.Position += head.Position - root.Transform(headInRoot).Position;

                _behaviourRig.SetBehaviourSpace(root);
            }

            _snapTurned = shouldSnapTurn;
        }

        private float _smoothAxis = 0f;
        private float _smoothVelocity = 0f;

        private void SmoothTurnSolve(float axis)
        {
            axis = Mathf.Clamp01((Mathf.Abs(axis) - 0.5f) * 2f) * Mathf.Sign(axis);

            _smoothAxis = Mathf.SmoothDamp(_smoothAxis, axis, ref _smoothVelocity, 0.05f);

            if (Mathf.Abs(_smoothAxis) > 0.01f)
            {
                var root = _behaviourRig.GetBehaviourSpace();
                var head = _behaviourRig.GetLocalHead();

                var headInRoot = root.InverseTransform(head);

                root.Rotation = Quaternion.AngleAxis(Time.deltaTime * _smoothAxis * 50f * _smoothTurnSpeed, root.Up) * root.Rotation;

                root.Position += head.Position - root.Transform(headInRoot).Position;

                _behaviourRig.SetBehaviourSpace(root);
            }
        }
    }
}
