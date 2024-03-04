using log4net.Util;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Integumentary;
using VAT.Avatars.REWORK;
using VAT.Interaction;
using VAT.Shared.Data;

namespace VAT.Characters
{
    public class AvatarGrabberPoint : IGrabPoint
    {
        public IHandGroup hand;
        public float radius;

        public SimpleTransform GetDefaultGrabPoint()
        {
            return GetGrabPoint(Vector2.up);
        }

        public SimpleTransform GetGrabCenter()
        {
            var grabPoint = GetDefaultGrabPoint();
            Vector3 direction = Vector3.Lerp(GetGrabNormal(), grabPoint.forward, 0.5f);
            return SimpleTransform.Create((Vector3)grabPoint.position + (direction * radius), grabPoint.rotation);
        }

        public Vector3 GetGrabNormal()
        {
            return -hand.Hand.Transform.up;
        }

        public SimpleTransform GetGrabPoint(Vector2 position)
        {
            return hand.GetPointOnPalm(position);
        }

        public SimpleTransform GetParentTransform()
        {
            return hand.Hand.Transform;
        }
    }
}
