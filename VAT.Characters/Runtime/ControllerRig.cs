using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars;
using VAT.Avatars.Example;
using VAT.Avatars.Integumentary;
using VAT.Input;
using VAT.Shared.Data;

namespace VAT.Characters
{
    public abstract class ControllerRig : CrystRig {
        [Header("References")]
        [SerializeField] protected Transform _leftWrist;

        [SerializeField] protected Transform _rightWrist;

        [SerializeField] protected Transform _head;

        [SerializeField] protected Transform vrRoot;

        public Camera cameraTest;

        public override void OnRegisterManager(ICrystRigManager rigManager)
        {
            base.OnRegisterManager(rigManager);

            var vitals = rigManager.GetVitalsOrDefault();

            if (vitals != null)
            {
                vitals.OnUpdatedVitals += OnUpdatedVitals;
            }
        }

        public override void OnDeregisterManager(ICrystRigManager rigManager)
        {
            base.OnDeregisterManager(rigManager);

            var vitals = rigManager.GetVitalsOrDefault();

            if (vitals != null)
            {
                vitals.OnUpdatedVitals -= OnUpdatedVitals;
            }
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

        public override bool TryGetTrackedRig(out CrystRig rig)
        {
            rig = this;
            return true;
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
                    arm = new BasicAvatarRig.BasicArm(SimpleTransform.Create(transform).InverseTransform(SimpleTransform.Create(_leftWrist)));
                    return true;
                case Handedness.RIGHT:
                    arm = new BasicAvatarRig.BasicArm(SimpleTransform.Create(transform).InverseTransform(SimpleTransform.Create(_rightWrist)));
                    return true;
            }
        }
    }
}
