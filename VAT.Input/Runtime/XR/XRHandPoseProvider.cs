using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.XR.Hands;
using VAT.Avatars;

namespace VAT.Input
{
    public class XRHandPoseProvider : IXRPoseProvider
    {
        private readonly Handedness _handedness;
        private readonly XRHandSubsystem _subsystem;

        private readonly HandPoseData _handPoseData;

        public XRHandPoseProvider(Handedness handedness, XRHandSubsystem subsystem) {
            _handedness = handedness;
            _subsystem = subsystem;

            _handPoseData = new HandPoseData()
            {
                fingers = HandPoseCreator.CreateFingers(4, 3),
                thumbs = HandPoseCreator.CreateThumbs(1, 2)
            };
        }

        private UnityEngine.XR.Hands.XRHand GetHand()
        {
            return _handedness switch
            {
                Handedness.RIGHT => _subsystem.rightHand,
                _ => _subsystem.leftHand,
            };
        }

        private float GetCurl(XRHandJoint parentJoint, XRHandJoint joint)
        {
            parentJoint.TryGetPose(out var parentPose);

            joint.TryGetPose(out var pose);
            float angle = Quaternion.Angle(parentPose.rotation, pose.rotation);

            return angle / 90f;
        }

        public HandPoseData GetHandPose()
        {
            var hand = GetHand();

            var rootPose = hand.rootPose;

            HandPoseCreator.SetCurls(_handPoseData.fingers[0].phalanges, GetCurl(hand.GetJoint(XRHandJointID.IndexProximal), hand.GetJoint(XRHandJointID.IndexIntermediate)));
            HandPoseCreator.SetCurls(_handPoseData.fingers[1].phalanges, GetCurl(hand.GetJoint(XRHandJointID.MiddleProximal), hand.GetJoint(XRHandJointID.MiddleIntermediate)));
            HandPoseCreator.SetCurls(_handPoseData.fingers[2].phalanges, GetCurl(hand.GetJoint(XRHandJointID.RingProximal), hand.GetJoint(XRHandJointID.RingIntermediate)));
            HandPoseCreator.SetCurls(_handPoseData.fingers[3].phalanges, GetCurl(hand.GetJoint(XRHandJointID.LittleProximal), hand.GetJoint(XRHandJointID.LittleIntermediate)));

            HandPoseCreator.SetCurls(_handPoseData.thumbs[0].phalanges, GetCurl(hand.GetJoint(XRHandJointID.ThumbProximal), hand.GetJoint(XRHandJointID.ThumbDistal)));

            return _handPoseData;
        }

        public bool IsValid()
        {
            return _subsystem != null && GetHand().isTracked;
        }
    }
}
