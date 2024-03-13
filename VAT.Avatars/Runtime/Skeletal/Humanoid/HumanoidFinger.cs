using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.XR;
using VAT.Avatars.Proportions;
using VAT.Avatars.REWORK;
using VAT.Shared.Data;
using VAT.Shared.Extensions;
using VAT.Shared.Utilities;

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

        public bool isLeft;

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

            float lerp = Time.deltaTime * 20f;

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
        }

        public override void Solve()
        {
            CalculateIKTargets();
        }
    }
}
