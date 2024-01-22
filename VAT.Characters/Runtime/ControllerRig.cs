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

        public override bool TryGetTrackedRig(out CrystRig rig)
        {
            rig = this;
            return true;
        }

        public override bool TryGetHead(out IJoint head)
        {
            var simpleTransform = SimpleTransform.Create(transform).InverseTransform(_head);
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
                    arm = new BasicAvatarRig.BasicArm(SimpleTransform.Create(transform).InverseTransform(_leftWrist));
                    return true;
                case Handedness.RIGHT:
                    arm = new BasicAvatarRig.BasicArm(SimpleTransform.Create(transform).InverseTransform(_rightWrist));
                    return true;
            }
        }
    }
}
