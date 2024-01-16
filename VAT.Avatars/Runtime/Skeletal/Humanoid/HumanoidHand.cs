using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars.Data;
using VAT.Avatars.Proportions;

namespace VAT.Avatars.Skeletal
{
    public class HumanoidHand : DataBoneGroup
    {
        public override int BoneCount => 1;

        public DataBone Hand => Bones[0];

        private int _fingerCount = 0;
        private HumanoidFinger[] _fingers;
        public override DataBoneGroup[] SubGroups => _fingers;
        public HumanoidFinger[] Fingers => _fingers;

        public override int SubGroupCount => _fingerCount;

        public HumanoidHand() {
            _fingers = Array.Empty<HumanoidFinger>();

            Initiate();
        }

        public HumanoidHand(DataBoneGroup parent) : this() {
            Attach(parent);
        }

        public PoseData GetData() {
            var poseData = new PoseData() {
                fingers = new FingerPose[_fingerCount]
            };

            for (var i = 0; i < _fingerCount; i++) {
                poseData.fingers[i] = _fingers[i].GetPose();
            }

            return poseData;
        }

        public void SetData(PoseData data) {
            for (var i = 0; i < _fingerCount; i++) {
                _fingers[i].SetTarget(data.fingers[i].point);
            }
        }

        public void WriteProportions(HandProportions proportions) {
            _fingerCount = proportions.FingerCount;
            _fingers = new HumanoidFinger[_fingerCount];

            for (var i = 0; i < _fingerCount; i++) {
                var finger = new HumanoidFinger();
                finger.Initiate();
                finger.Attach(this);
                finger.WriteProportions(proportions.fingerProportions[i], proportions);
                _fingers[i] = finger;
            }

            if (_fingerCount > 0)
                _fingers[0].defaultRotation = proportions.fingerProportions[0].metaCarpalTransform.rotation;
        }

        public override void Solve()
        {
            for (var i = 0; i < _fingerCount; i++) {
                _fingers[i].Solve();
            }
        }
    }
}
