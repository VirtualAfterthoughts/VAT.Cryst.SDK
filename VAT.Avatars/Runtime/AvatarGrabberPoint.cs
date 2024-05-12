using UnityEngine;

using VAT.Avatars.Bones;
using VAT.Interaction;
using VAT.Shared.Data;

namespace VAT.Characters
{
    public class AvatarGrabberPoint : IPalmPoint
    {
        public IHandGroup hand;
        public float radius;

        public SimpleTransform GetDefaultPoint()
        {
            return GetPoint(Vector2.up);
        }

        public SimpleTransform GetProximityCenter()
        {
            var grabPoint = GetDefaultPoint();
            Vector3 direction = Vector3.Lerp(GetNormal(), grabPoint.forward, 0.5f);
            return SimpleTransform.Create((Vector3)grabPoint.position + (direction * radius), grabPoint.rotation);
        }

        public Vector3 GetNormal()
        {
            return -hand.Hand.Transform.up;
        }

        public SimpleTransform GetPoint(Vector2 position)
        {
            return hand.GetPointOnPalm(position);
        }

        public SimpleTransform GetHostTransform()
        {
            return hand.Hand.Transform;
        }
    }
}
