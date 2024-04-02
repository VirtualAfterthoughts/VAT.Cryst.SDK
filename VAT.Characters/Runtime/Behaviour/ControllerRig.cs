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
    public struct BasicHand : IHand
    {
        public SimpleTransform Transform { get { return _transform; } set { _transform = value; } }

        private SimpleTransform _transform;

        public BasicHand(SimpleTransform transform)
        {
            _transform = transform;
        }

        public IInputController GetInputControllerOrNull()
        {
            return default;
        }

        public IInputHand GetInputHandOrNull()
        {
            return default;
        }
    }

    public struct BasicArm : IArm
    {
        private IJoint[] _bones;

        public BasicArm(params SimpleTransform[] transforms)
        {
            var bones = new IJoint[transforms.Length];

            for (var i = 0; i < bones.Length; i++)
            {
                if (i <= 0)
                {
                    bones[i] = new BasicHand(transforms[i]);
                    continue;
                }

                bones[i] = new BasicJoint(transforms[i]);
            }

            _bones = bones;
        }

        public readonly int JointCount => 1;

        public readonly IJoint[] Joints => _bones;

        public void SetJoint(int index, IJoint joint)
        {
            _bones[index] = joint;
        }

        public IJoint GetElbowOrNull()
        {
            if (_bones.Length > 1)
            {
                return _bones.ElementAt(1);
            }

            return null;
        }

        public IHand GetHandOrNull()
        {
            return _bones.ElementAt(0) as IHand;
        }

        public IJoint GetUpperArmOrNull()
        {
            if (_bones.Length > 2)
            {
                return _bones.ElementAt(2);
            }

            return null;
        }
    }

    public abstract class ControllerRig : CrystRig, IBehaviourRig {
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
            var vitals = RigManager.GetVitalsOrNull();

            if (vitals != null)
            {
                vitals.OnUpdatedVitals += OnUpdatedVitals;
            }

            _crouching.OnRegister(this);
            _turning.OnRegister(this);
        }

        public override void OnRigDisable()
        {
            var vitals = RigManager.GetVitalsOrNull();

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

        public override bool TryGetTrackedRig(out CrystRig rig)
        {
            rig = this;
            return true;
        }

        public SimpleTransform GetLocalHead()
        {
            TryGetHead(out var head);
            return head.Transform;
        }

        public override bool TryGetHead(out IJoint head)
        {
            var simpleTransform = SimpleTransform.Create(transform).InverseTransform(SimpleTransform.Create(_head));
            simpleTransform.lossyScale = Vector3.one;

            head = new BasicJoint(simpleTransform);
            return true;
        }

        public override bool TryGetArm(Handedness handedness, out IArm arm)
        {
            switch (handedness)
            {
                default:
                    arm = default;
                    return false;
                case Handedness.LEFT:
                    arm = new BasicArm(SimpleTransform.Create(transform).InverseTransform(SimpleTransform.Create(_leftWrist)));
                    return true;
                case Handedness.RIGHT:
                    arm = new BasicArm(SimpleTransform.Create(transform).InverseTransform(SimpleTransform.Create(_rightWrist)));
                    return true;
            }
        }

        public SimpleTransform GetRoot()
        {
            return SimpleTransform.Create(transform.position, transform.rotation);
        }

        public SimpleTransform GetBehaviourSpace()
        {
            return SimpleTransform.Create(vrRoot.localPosition, vrRoot.localRotation);
        }

        public void SetBehaviourSpace(SimpleTransform transform)
        {
            vrRoot.SetLocalPositionAndRotation(transform.position, transform.rotation);
        }

        public IHand GetPrimaryHand()
        {
            if (TryGetArm(Handedness.RIGHT, out var arm))
            {
                return arm.GetHandOrNull();
            }

            return null;
        }

        public IHand GetSecondaryHand()
        {
            if (TryGetArm(Handedness.LEFT, out var arm))
            {
                return arm.GetHandOrNull();
            }

            return null;
        }
    }
}
