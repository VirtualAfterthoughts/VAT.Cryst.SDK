using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Shared.Data;
using VAT.Shared.Math;

namespace VAT.Interaction
{
    public class BoxGrip : Grip
    {
        public BoxCollider boxCollider;

        public override SimpleTransform GetTargetInWorld(IInteractor interactor)
        {
            var grabPoint = interactor.GetGrabPoint();
            var localGrabPoint = boxCollider.transform.InverseTransformPoint(grabPoint.position);

            var face = Geometry.ClosestFace(localGrabPoint, boxCollider.center, boxCollider.size, Faces.EVERYTHING);
            if (face.HasValue)
            {
                var worldPoint = boxCollider.transform.TransformPoint(face.Value.ClosestPoint(localGrabPoint));
                var worldNormal = boxCollider.transform.TransformDirection(face.Value.normal);

                var grabRotation = Quaternion.FromToRotation(interactor.GetRigidbody().transform.up, worldNormal) * grabPoint.rotation;

                return SimpleTransform.Create(worldPoint, grabRotation);
            }

            return grabPoint;
        }
    }
}
