using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input.Data
{
    public static class HandPoseHelper
    {
        public static readonly HandPoseData DefaultClosedPose = CreatePose(1f);

        public static HandPoseData DefaultOpenPose = CreatePose(1f);

        private static HandPoseData CreatePose(float curl)
        {
            var fingers = HandPoseCreator.CreateFingers();
            var thumbs = HandPoseCreator.CreateThumbs();

            foreach (var finger in fingers)
            {
                HandPoseCreator.SetCurls(finger.phalanges, curl);
            }

            foreach (var thumb in thumbs)
            {
                HandPoseCreator.SetCurls(thumb.phalanges, curl);
            }

            return new HandPoseData()
            {
                fingers = fingers,
                thumbs = thumbs,
            };
        }
    }
}
