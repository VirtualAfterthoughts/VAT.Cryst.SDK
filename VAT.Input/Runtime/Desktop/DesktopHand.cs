using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Avatars;
using VAT.Input.Desktop;

namespace VAT.Input
{
    public class DesktopHand : IInputHand
    {
        private readonly DesktopController _controller;

        private HandPoseData _handPose;

        public DesktopHand(DesktopController controller)
        {
            _controller = controller;

            _handPose = new HandPoseData()
            {
                fingers = HandPoseCreator.CreateFingers(),
                thumbs = HandPoseCreator.CreateThumbs(),
            };
        }

        public void Update()
        {
            _controller.TryGetGrip(out var grip);

            float curl = grip.GetAxis();

            for (var i = 0; i < _handPose.fingers.Length; i++)
            {
                HandPoseCreator.SetCurls(_handPose.fingers[i].phalanges, curl);
            }

            for (var i = 0; i < _handPose.thumbs.Length; i++)
            {
                HandPoseCreator.SetCurls(_handPose.thumbs[i].phalanges, curl);
            }
        }

        public HandPoseData GetHandPose()
        {
            return _handPose;
        }
    }
}
