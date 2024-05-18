using UnityEngine;

using VAT.Avatars.Bones;
using VAT.Input.Data;
using VAT.Interaction;
using VAT.Shared.Data;

namespace VAT.Characters
{
    public class AvatarGrabberPoint : PalmPoint
    {
        public IHandGroup hand;
        public float radius;

        public override SimpleTransform GetProximityCenterInHost()
        {
            var grabPoint = GetUpperPalmInHost();
            Vector3 direction = Vector3.Lerp(GetNormalInHost(), grabPoint.forward, 0.5f);
            return SimpleTransform.Create((Vector3)grabPoint.position + (direction * radius), grabPoint.rotation);
        }

        public override Vector3 GetNormalInHost()
        {
            return Vector3.down;
        }

        public override SimpleTransform GetPalmInHost(Vector2 position)
        {
            return GetHostTransform().InverseTransform(hand.GetPointOnPalm(position));
        }

        public override SimpleTransform GetPressureCenterInHost(HandPoseData pose)
        {
            return GetPalmInHost(pose.centerOfPressure);
        }

        public override SimpleTransform GetHostTransform()
        {
            return hand.Hand.Transform;
        }
    }
}
