using log4net.Util;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Integumentary;
using VAT.Interaction;
using VAT.Shared.Data;

namespace VAT.Characters
{
    public class AvatarGrabberPoint : IGrabberPoint
    {
        public AvatarArm arm;
        public float radius;

        public SimpleTransform GetDefaultGrabPoint()
        {
            return GetGrabPoint(Vector2.up);
        }

        public SimpleTransform GetGrabCenter()
        {
            var grabPoint = GetDefaultGrabPoint();
            Vector3 direction = Vector3.Lerp(-GetGrabNormal(), grabPoint.forward, 0.5f);
            return SimpleTransform.Create((Vector3)grabPoint.position + (direction * radius), grabPoint.rotation);
        }

        public Vector3 GetGrabNormal()
        {
            return -arm.PhysArm.Hand.Hand.Transform.up;
        }

        public SimpleTransform GetGrabPoint(Vector2 position)
        {
            return arm.PhysArm.Hand.GetPointOnPalm(position);
        }

        public SimpleTransform GetParentTransform()
        {
            return arm.PhysArm.Hand.Hand.Transform;
        }
    }
}
