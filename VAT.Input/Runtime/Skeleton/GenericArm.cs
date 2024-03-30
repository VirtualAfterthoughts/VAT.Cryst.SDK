using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VAT.Input.Skeleton
{
    public readonly struct GenericArm : IArm
    {
        private readonly IJoint[] _joints;
        public readonly IJoint[] Joints => _joints;

        public readonly int JointCount => Joints.Length;

        private readonly IHand _hand;

        public GenericArm(IHand hand, params IJoint[] joints)
        {
            _joints = new IJoint[joints.Length + 1];
            _joints[0] = hand;

            for (var i = 0; i < joints.Length; i++)
            {
                _joints[i + 1] = joints[i];
            }

            _hand = hand;
        }

        public readonly IHand GetHandOrNull()
        {
            return _hand;
        }

        public readonly IJoint GetElbowOrNull()
        {
            return Joints.ElementAtOrDefault(1);
        }

        public readonly IJoint GetUpperArmOrNull()
        {
            return Joints.ElementAtOrDefault(2);
        }
    }
}
