using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars.Data;
using VAT.Avatars.Proportions;
using VAT.Avatars.REWORK;
using VAT.Input;
using VAT.Shared.Extensions;

namespace VAT.Avatars.Skeletal
{
    public class HumanoidHand : DataBoneGroup, IHandGroup
    {
        public override int BoneCount => 1;

        public DataBone Hand => Bones[0];

        public DataBone Palm = null;

        private Handedness _handedness = Handedness.LEFT;

        private HandProportions _proportions;

        private int _fingerCount = 0;
        private HumanoidFinger[] _fingers;
        public override DataBoneGroup[] SubGroups => _fingers;
        public HumanoidFinger[] Fingers => _fingers;

        public override int SubGroupCount => _fingerCount;

        IBone IHandGroup.Hand => Hand;

        IBone IHandGroup.Palm => Palm;

        public HumanoidHand() {
            _fingers = Array.Empty<HumanoidFinger>();

            Initiate();
        }

        public HumanoidHand(DataBoneGroup parent) : this() {
            Attach(parent);
        }

        public override void Initiate()
        {
            base.Initiate();

            Palm = new DataBone(Hand);
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
            _proportions = proportions;

            _handedness = proportions.handedness;

            Palm.localPosition = 0.7f * proportions.wristEllipsoid.height * Vector3.forward + Vector3.down * proportions.wristEllipsoid.radius.y;

            float angle;
            if (_handedness == Handedness.LEFT)
                angle = -90f;
            else
                angle = 90f;

            Palm.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

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

        public Vector3 GetPalmSize()
        {
            var radius = _proportions.knuckleEllipsoid.radius;
            var height = _proportions.wristEllipsoid.height;
            Vector3 size = new(0f, radius.x, height * 0.5f);

            return size;
        }

#if UNITY_EDITOR
        public override void DrawGizmos()
        {
            base.DrawGizmos();

            using var color = TempGizmoColor.Create();

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Palm.position, 0.005f);

            using (var matrix = TempGizmoMatrix.Create())
            {
                Gizmos.matrix = Palm.Transform.localToWorldMatrix;

                Gizmos.DrawWireCube(Vector3.zero, GetPalmSize());
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Palm.position, Palm.position + Palm.forward * 0.1f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(Palm.position, Palm.position + Palm.up * 0.1f);
        }
#endif
    }
}
