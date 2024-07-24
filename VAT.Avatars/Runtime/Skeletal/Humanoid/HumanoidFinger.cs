using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;
using VAT.Avatars.Proportions;
using VAT.Avatars.Bones;
using VAT.Shared.Data;
using VAT.Input.Data;
using VAT.Shared.Extensions;
using UnityEngine.UIElements;
using VAT.Cryst.Math;

namespace VAT.Avatars.Skeletal
{
    public class HumanoidFinger : DataBoneGroup, IFingerGroup
    {
        private int _boneCount = 5;
        public override int BoneCount => _boneCount;

        private DataBone _hand;

        public DataBone MetaCarpal => Bones[0];
        public DataBone Proximal => Bones[1];
        public DataBone Middle => Bones[2];
        public DataBone Distal => Bones[3];
        public DataBone End => Bones[4];

        private HandProportions _handProportions;
        private FingerProportions _proportions;

        private float _proximalLength;
        private float _middleLength;
        private float _distalLength;

        public SimpleTransform NeutralEndBone => _hand.Transform.Transform(defaultEnd);

        IBone IFingerGroup.Proximal => Proximal;

        IBone IFingerGroup.Middle => Middle;

        IBone IFingerGroup.Distal => Distal;

        public SimpleTransform defaultEnd;

        public quaternion defaultRotation = quaternion.identity;

        public SimpleTransform target = SimpleTransform.Default;

        public bool isLeft;

        public SimpleTransform offsetHand = SimpleTransform.Default;
        public bool shouldOffset = false;

        private SimpleTransform _lastOffsetHand = SimpleTransform.Default;

        public override void Initiate()
        {
            base.Initiate();

            openPose = FingerPoseData.Create(3);
            closedPose = FingerPoseData.Create(3);
            blendPose = FingerPoseData.Create(3);
        }

        public override void BindPose() {
            base.BindPose();

            MetaCarpal.localPosition = _proportions.metaCarpalTransform.position;
            MetaCarpal.localRotation = _proportions.metaCarpalTransform.rotation;

            Proximal.localPosition = _proportions.proximalTransform.position;
            Proximal.localRotation = _proportions.proximalTransform.rotation;

            Middle.localRotation = _proportions.middleTransform.rotation;

            _proximalLength = _proportions.proximalEllipsoid.height;
            _middleLength = _proportions.middleEllipsoid.height;
            _distalLength = _proportions.distalEllipsoid.height;

            Middle.localPosition = Vector3.forward * _proximalLength;
            Distal.localPosition = Vector3.forward * _middleLength;
            End.localPosition = Vector3.forward * _distalLength;
        }

        public override void NeutralPose() {
            base.NeutralPose();

            MetaCarpal.localRotation = defaultRotation;
            Proximal.localRotation = quaternion.identity;
            Middle.localRotation = quaternion.identity;
            Distal.localRotation = quaternion.identity;
        }

        public override void Attach(DataBoneGroup group) {
            base.Attach(group);
            _hand = group.LastBone;
        }

        public void WriteProportions(FingerProportions proportions, HandProportions handProportions)
        {
            // Change BoneCount based on phalanx count to account for CCD solver
            // BoneCount is phalanx count + 1 (metacarpal) + 1 (end)
            _boneCount = proportions.phalanxCount + 2;

            _proportions = proportions;

            _handProportions = handProportions;
        }

        public FingerPoseData openPose;
        public FingerPoseData closedPose;
        public FingerPoseData blendPose;

        public static float Remap(float value)
        {
            float from1 = -1f;
            float to1 = 1f;

            float from2 = 0f;
            float to2 = 1f;

            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        private float GetSplayAngle(float splay)
        {
            return Mathf.LerpUnclamped(-30f, 30f, Remap(splay));
        }

        private float GetCurlAngle(float curl)
        {
            return Mathf.LerpUnclamped(-90f, 90f, Remap(curl));
        }

        private float _lastSplay = 0f;
        private float _lastCurl01 = 0f;
        private float _lastCurl02 = 0f;
        private float _lastCurl03 = 0f;

        public void CalculateIKTargets()
        {
            float leftMult = (isLeft ? -1f : 1f);

            Vector3 fwd = MetaCarpal.Parent.forward;
            Vector3 handUp = MetaCarpal.Parent.up;

            Proximal.rotation = Quaternion.LookRotation(fwd, handUp);

            // Calculate values
            float splay = Mathf.Lerp(openPose.splay, closedPose.splay, blendPose.phalanges[0].curl);
            float curl01 = Mathf.Lerp(openPose.phalanges[0].curl, closedPose.phalanges[0].curl, blendPose.phalanges[0].curl);
            float curl02 = Mathf.Lerp(openPose.phalanges[1].curl, closedPose.phalanges[1].curl, blendPose.phalanges[1].curl);
            float curl03 = Mathf.Lerp(openPose.phalanges[2].curl, closedPose.phalanges[2].curl, blendPose.phalanges[2].curl);

            float lerp = Smoothing.CalculateDecay(20f, Time.deltaTime);

            splay = Mathf.Lerp(_lastSplay, splay, lerp);
            curl01 = Mathf.Lerp(_lastCurl01, curl01, lerp);
            curl02 = Mathf.Lerp(_lastCurl02, curl02, lerp);
            curl03 = Mathf.Lerp(_lastCurl03, curl03, lerp);

            _lastSplay = splay;
            _lastCurl01 = curl01;
            _lastCurl02 = curl02;
            _lastCurl03 = curl03;

            // Splay
            Proximal.rotation = Quaternion.AngleAxis(-GetSplayAngle(splay) * leftMult, handUp) * Quaternion.LookRotation(fwd, handUp);

            // Curl 01
            Proximal.rotation = Quaternion.AngleAxis(GetCurlAngle(curl01), Proximal.right) * Proximal.rotation;

            // Curl 02
            Middle.localRotation = Quaternion.AngleAxis(GetCurlAngle(curl02), Vector3.right);

            // Curl 03
            Distal.localRotation = Quaternion.AngleAxis(GetCurlAngle(curl03), Vector3.right);

            target = MetaCarpal.Parent.Transform.InverseTransform(Distal.Transform);
        }

        private float _lastBlend = 0f;

        public override void Solve()
        {
            CalculateIKTargets();

            // Get solved rotations
            var solvedWorldProximal = Proximal.rotation;
            var solvedMiddle = Middle.localRotation;
            var solvedDistal = Distal.localRotation;

            // Solve trig ik
            var parent = MetaCarpal.Parent.Transform;
            var realParent = parent;

            if (shouldOffset)
            {
                float lerp = Smoothing.CalculateDecay(24f, Time.deltaTime);
                var newOffset = SimpleTransform.Create(Vector3.Slerp(_lastOffsetHand.position, offsetHand.position, lerp), Quaternion.Slerp(_lastOffsetHand.rotation, offsetHand.rotation, lerp));
                _lastOffsetHand = newOffset;

                parent = parent.Transform(newOffset);
            }

            var target = parent.Transform(this.target);

            Vector3 vector = target.position - Proximal.position;

            float a = vector.magnitude;
            float b = _proportions.proximalEllipsoid.height;
            float c = _proportions.middleEllipsoid.height;

            float A = Mathf.Acos(((Mathf.Pow(a, 2f) + Mathf.Pow(b, 2f) - Mathf.Pow(c, 2f)) / (2f * a * b)).SinClamp());
            float B = Mathf.Acos(((Mathf.Pow(b, 2f) + Mathf.Pow(c, 2f) - Mathf.Pow(a, 2f)) / (2f * b * c)).SinClamp());

            Proximal.rotation = Quaternion.LookRotation(vector, Quaternion.AngleAxis(-90f, Proximal.right) * vector);
            Proximal.rotation = Quaternion.AngleAxis(A * Mathf.Rad2Deg, -Proximal.right) * Proximal.rotation;

            Middle.rotation = Quaternion.AngleAxis(180f - B * Mathf.Rad2Deg, Proximal.right) * Proximal.rotation;

            // Reach
            var rightOffset = Quaternion.FromToRotation(target.right, Middle.right);
            var targetTip = target.position + target.forward * _distalLength;

            Distal.rotation = rightOffset * Quaternion.LookRotation(math.normalize(targetTip - Distal.position), target.up);

            // Blend for open pose (REPLACE IN FUTURE)
            float blendCurl = 1f - blendPose.phalanges[0].curl;

            blendCurl = Mathf.Lerp(_lastBlend, blendCurl, Smoothing.CalculateDecay(20f, Time.deltaTime));

            _lastBlend = blendCurl;

            var gripOffset = parent.rotation * Quaternion.Inverse(realParent.rotation);

            // If quaternion angle is > 180 degrees (w is negative) convert to shortened angle
            if (gripOffset.w < 0)
            {
                gripOffset.x = -gripOffset.x;
                gripOffset.y = -gripOffset.y;
                gripOffset.z = -gripOffset.z;
                gripOffset.w = -gripOffset.w;
            }

            gripOffset.ToAngleAxis(out var gripAngle, out var gripAxis);
            gripOffset = Quaternion.AngleAxis(gripAngle * 0.8f, gripAxis);

            Proximal.rotation = Quaternion.Slerp(Proximal.rotation, gripOffset * solvedWorldProximal, blendCurl);
            Middle.localRotation = Quaternion.Slerp(Middle.localRotation, solvedMiddle, blendCurl);
            Distal.localRotation = Quaternion.Slerp(Distal.localRotation, solvedDistal, blendCurl);
        }
    }
}
