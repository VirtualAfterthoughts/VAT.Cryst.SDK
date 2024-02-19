using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public class SphereGrip : TargetGrip
    {
        protected override IGripJoint OnCreateGripJoint(IInteractor interactor)
        {
            return new SphereGripJoint(GetTargetTransform(), GetWorldRadius());
        }

        private void Reset()
        {
            if (gameObject.TryGetComponent<SphereCollider>(out var sphereCollider))
            {
                Radius = sphereCollider.radius;
            }
            else
            {
                Radius = 0.5f;
            }
        }

        public override SimpleTransform GetTargetInWorld(IInteractor interactor)
        {
            var target = GetTargetTransform();
            var grabPoint = interactor.GetGrabPoint();
            var direction = ((Vector3)grabPoint.position - target.position).normalized;

            var grabRotation = Quaternion.FromToRotation(interactor.GetRigidbody().transform.up, direction) * grabPoint.rotation;

            return SimpleTransform.Create(target.position + direction * GetWorldRadius(), grabRotation);
        }
    }
}
