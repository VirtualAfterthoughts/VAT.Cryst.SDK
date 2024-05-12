using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VAT.Input.Skeleton
{
    public readonly struct GenericArm : IInputArm
    {
        private readonly IInputJoint[] _joints;
        public readonly IInputJoint[] Joints => _joints;

        public readonly int JointCount => Joints.Length;

        private readonly IInputHand _hand;

        public GenericArm(IInputHand hand, params IInputJoint[] joints)
        {
            _joints = new IInputJoint[joints.Length + 1];
            _joints[0] = hand;

            for (var i = 0; i < joints.Length; i++)
            {
                _joints[i + 1] = joints[i];
            }

            _hand = hand;
        }

        public readonly IInputHand GetHand()
        {
            return _hand;
        }

        public readonly IInputJoint GetElbow()
        {
            return Joints.ElementAtOrDefault(1);
        }

        public readonly IInputJoint GetUpperArm()
        {
            return Joints.ElementAtOrDefault(2);
        }
    }
}
