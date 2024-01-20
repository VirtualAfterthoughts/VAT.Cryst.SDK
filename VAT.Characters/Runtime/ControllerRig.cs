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
        [SerializeField] private Transform _leftWrist;
        [SerializeField] private Transform _rightWrist;
        [SerializeField] private Transform _head;

        public override bool TryGetTrackedRig(out CrystRig rig)
        {
            rig = this;
            return true;
        }

        public override bool TryGetHead(out IJoint head)
        {
            head = new BasicJoint(SimpleTransform.Create(transform).InverseTransform(_head));
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
