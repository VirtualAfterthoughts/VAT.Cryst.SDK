using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using VAT.Avatars;
using VAT.Avatars.Integumentary;
using VAT.Input;
using VAT.Input.Skeleton;
using VAT.Shared.Data;

namespace VAT.Characters
{
    public abstract class ControllerRig : CrystRig, IBehaviourRig
    {
        [Header("References")]
        [SerializeField] protected Transform _leftWrist;

        [SerializeField] protected Transform _rightWrist;

        [SerializeField] protected Transform _head;

        [SerializeField] protected Transform vrRoot;

        [SerializeField]
        protected CrouchBehaviour _crouching;

        [SerializeField]
        protected TurnBehaviour _turning;

        public Camera cameraTest;

        public override void OnRigEnable()
        {
            var vitals = RigManager.GetVitals();

            if (vitals != null)
            {
                vitals.OnUpdatedVitals += OnUpdatedVitals;
            }

            _crouching.OnRegister(this);
            _turning.OnRegister(this);
        }

        public override void OnRigDisable()
        {
            var vitals = RigManager.GetVitals();

            if (vitals != null)
            {
                vitals.OnUpdatedVitals -= OnUpdatedVitals;
            }

            _crouching.OnDeregister(this);
            _turning.OnDeregister(this);
        }

        private void OnUpdatedVitals(ICrystVitals vitals)
        {
            float scale = (vitals.CharacterMeasurements.height / vitals.PlayerMeasurements.height);
            vrRoot.localScale = scale * Vector3.one;

            if (cameraTest != null)
            {
                cameraTest.nearClipPlane = 0.01f * scale;
                cameraTest.farClipPlane = 1000f * scale;
            }
        }

        public sealed override bool TryGetInput(out IBasicInput input)
        {
            var movement = OnProcessMovement();
            var jump = OnProcessJump();

            input = new GenericInput(movement, jump);
            return true;
        }

        public override void OnLateUpdate(float deltaTime)
        {
            OnProcessTracking();

            _crouching.Solve();
            _turning.Solve();
        }

        protected abstract void OnProcessTracking();

        protected abstract Vector3 OnProcessMovement();

        protected abstract bool OnProcessJump();

        public SimpleTransform GetLocalHead()
        {
            TryGetHead(out var head);
            return head.Transform;
        }

        public override bool TryGetHead(out IInputJoint head)
        {
            var simpleTransform = SimpleTransform.Create(transform.position, transform.rotation).InverseTransform(SimpleTransform.Create(_head.position, _head.rotation));

            head = new BasicJoint(simpleTransform);
            return true;
        }

        public override bool TryGetArm(Handedness handedness, out IInputArm arm)
        {
            var root = GetRoot();

            switch (handedness)
            {
                default:
                    arm = default;
                    return false;
                case Handedness.LEFT:
                    arm = new GenericArm(new GenericHand(root.InverseTransform(SimpleTransform.Create(_leftWrist.position, _leftWrist.rotation)), null));
                    return true;
                case Handedness.RIGHT:
                    arm = new GenericArm(new GenericHand(root.InverseTransform(SimpleTransform.Create(_rightWrist.position, _rightWrist.rotation)), null));
                    return true;
            }
        }

        public SimpleTransform GetRoot()
        {
            return SimpleTransform.Create(transform.position, transform.rotation, transform.lossyScale);
        }

        public void SetRoot(SimpleTransform root)
        {
            transform.SetPositionAndRotation(root.position, root.rotation);
        }

        public SimpleTransform GetBehaviourSpace()
        {
            return SimpleTransform.Create(vrRoot.localPosition, vrRoot.localRotation, vrRoot.lossyScale);
        }

        public void SetBehaviourSpace(SimpleTransform transform)
        {
            vrRoot.SetLocalPositionAndRotation(transform.position, transform.rotation);
        }

        public IInputHand GetPrimaryHand()
        {
            if (TryGetArm(Handedness.RIGHT, out var arm))
            {
                return arm.GetHand();
            }

            return null;
        }

        public IInputHand GetSecondaryHand()
        {
            if (TryGetArm(Handedness.LEFT, out var arm))
            {
                return arm.GetHand();
            }

            return null;
        }
    }
}
