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
    public class HumanoidThumb : DataBoneGroup, IThumbGroup
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

        IBone IThumbGroup.Proximal => Proximal;

        IBone IThumbGroup.Middle => Middle;

        IBone IThumbGroup.Distal => Distal;

        public SimpleTransform defaultEnd;

        public quaternion defaultRotation = quaternion.identity;

        public bool isLeft;

        public override void Initiate()
        {
            base.Initiate();

            openPose = ThumbPoseData.Create(2);
            closedPose = ThumbPoseData.Create(2);
            blendPose = ThumbPoseData.Create(2);
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

        public ThumbPoseData openPose;
        public ThumbPoseData closedPose;
        public ThumbPoseData blendPose;

        public static float Remap(float value)
        {
            float from1 = -1f;
            float to1 = 1f;

            float from2 = 0f;
            float to2 = 1f;

            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public void CalculateIKTargets()
        {
            float leftMult = (isLeft ? -1f : 1f);

            MetaCarpal.localRotation = Quaternion.AngleAxis(90f * leftMult, Vector3.forward);
            MetaCarpal.rotation = Quaternion.AngleAxis(Mathf.Lerp(0f, -60f, Remap(Mathf.Lerp(openPose.stretched, closedPose.stretched, blendPose.phalanges[0].curl))) * leftMult, MetaCarpal.up) * MetaCarpal.rotation;
            MetaCarpal.rotation = Quaternion.AngleAxis(Mathf.Lerp(10f, -60f, Remap(Mathf.Lerp(openPose.spread, closedPose.spread, blendPose.phalanges[0].curl))), MetaCarpal.right) * MetaCarpal.rotation;
            MetaCarpal.rotation = Quaternion.AngleAxis(Mathf.Lerp(-20f, 60f, Remap(Mathf.Lerp(openPose.twist, closedPose.twist, blendPose.phalanges[0].curl))) * leftMult, MetaCarpal.forward) * MetaCarpal.rotation;

            Proximal.localRotation = Quaternion.identity;
            Middle.localRotation = Quaternion.AngleAxis(Mathf.Lerp(-90f, 90f, Remap(Mathf.Lerp(openPose.phalanges[0].curl, closedPose.phalanges[0].curl, blendPose.phalanges[0].curl))), Vector3.right);
            Distal.localRotation = Quaternion.AngleAxis(Mathf.Lerp(-90f, 90f, Remap(Mathf.Lerp(openPose.phalanges[1].curl, closedPose.phalanges[1].curl, blendPose.phalanges[1].curl))), Vector3.right);
        }

        public override void Solve()
        {
            CalculateIKTargets();
        }
    }
}
