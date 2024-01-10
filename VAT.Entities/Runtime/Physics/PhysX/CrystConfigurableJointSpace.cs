using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Entities.PhysX
{
    public sealed class CrystConfigurableJointSpace : CrystJointSpace {
        private readonly ConfigurableJoint _joint;
        private readonly ConfigurableJointSpace _space;

        public override float3 RawTargetPosition { get => _joint.targetPosition; set => _joint.targetPosition = value; }
        public override quaternion RawTargetRotation { get => _joint.targetRotation; set => _joint.targetRotation = value; }
        public override float3 RawTargetVelocity { get => _joint.targetVelocity; set => _joint.targetVelocity = value; }
        public override float3 RawTargetAngularVelocity { get => _joint.targetAngularVelocity; set => _joint.targetAngularVelocity = value; }

        public CrystConfigurableJointSpace(ConfigurableJoint joint) {
            _joint = joint;
            _space = new ConfigurableJointSpace(joint);
        }

        public override float3 InverseTransformTargetPosition(float3 target, CrystSpace space = CrystSpace.WORLD) {
            return space switch
            {
                CrystSpace.LOCAL => _space.GetTargetPositionLocal(target),
                _ => _space.GetTargetPositionWorld(target),
            };
        }

        public override quaternion InverseTransformTargetRotation(quaternion target, CrystSpace space = CrystSpace.WORLD)
        {
            return space switch
            {
                CrystSpace.LOCAL => _space.GetTargetRotationLocal(target),
                _ => _space.GetTargetRotationWorld(target),
            };
        }
    }
}
