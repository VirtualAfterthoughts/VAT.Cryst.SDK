using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public class SphereGripJoint : GripJoint
    {
        private Transform _center;
        private float _radius;

        public SphereGripJoint(Transform center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        protected override void OnAttachJoint(ConfigurableJoint joint)
        {
            var grabPoint = Interactor.GetGrabPoint();
            var grabNormal = Interactor.GetRigidbody().transform.up;

            joint.SetWorldAnchor((Vector3)grabPoint.position - grabNormal * _radius);
            joint.SetWorldConnectedAnchor(_center.position);
        }

        protected override void OnLockJoint()
        {
            base.OnLockJoint();

            Joint.angularXMotion = Joint.angularYMotion = Joint.angularZMotion = ConfigurableJointMotion.Free;
            Joint.slerpDrive = new JointDrive();
        }
    }
}
