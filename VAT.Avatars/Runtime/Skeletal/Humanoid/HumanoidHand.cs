using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using VAT.Avatars.Proportions;
using VAT.Avatars.REWORK;
using VAT.Input;
using VAT.Shared.Data;
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

        private int _thumbCount = 0;
        private HumanoidThumb[] _thumbs;

        private new DataBoneGroup[] _subGroups = null;
        public override DataBoneGroup[] SubGroups => _subGroups;
        public HumanoidFinger[] Fingers => _fingers;

        public override int SubGroupCount => _fingerCount + _thumbCount;

        IBone IHandGroup.Hand => Hand;

        IBone IHandGroup.Palm => Palm;

        IFingerGroup[] IHandGroup.Fingers => _fingers;

        IThumbGroup[] IHandGroup.Thumbs => _thumbs;

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

            _thumbCount = proportions.ThumbCount;
            _thumbs = new HumanoidThumb[_thumbCount];

            for (var i = 0; i < _fingerCount; i++) {
                var finger = new HumanoidFinger();
                finger.Initiate();
                finger.Attach(this);
                finger.WriteProportions(proportions.fingerProportions[i], proportions);
                finger.isLeft = _handedness == Handedness.LEFT;
                _fingers[i] = finger;
            }

            for (var i = 0; i < _thumbCount; i++)
            {
                var thumb = new HumanoidThumb();
                thumb.Initiate();
                thumb.Attach(this);
                thumb.WriteProportions(proportions.thumbProportions[i], proportions);
                thumb.isLeft = _handedness == Handedness.LEFT;
                thumb.defaultRotation = proportions.thumbProportions[i].metaCarpalTransform.rotation;
                _thumbs[i] = thumb;
            }

            _subGroups = _fingers.Concat<DataBoneGroup>(_thumbs).ToArray();
        }

        public override void Solve()
        {
            for (var i = 0; i < _fingerCount; i++) {
                _fingers[i].Solve();
            }

            for (var i = 0; i < _thumbCount; i++)
            {
                _thumbs[i].Solve();
            }
        }

        public Vector3 GetPalmSize()
        {
            var radius = _proportions.knuckleEllipsoid.radius;
            var height = _proportions.wristEllipsoid.height;
            Vector3 size = new(0f, radius.x * 1.25f, height * 0.5f);

            return size;
        }

        public SimpleTransform GetPointOnPalm(Vector2 position)
        {
            var size = GetPalmSize();
            var size2D = new Vector2(size.y, size.z) * 0.5f;

            var palmPos = Palm.position + Palm.forward * size2D.y * position.y + Palm.up * size2D.x * position.x;
            return SimpleTransform.Create(palmPos, Palm.rotation);
        }

        public void SetOpenPose(HandPoseData data)
        {
            var fingers = data.RemapFingers(_fingerCount);
            var thumbs = data.RemapThumbs(_thumbCount);

            for (var i = 0; i < _fingerCount; i++)
            {
                Fingers[i].openPose = fingers[i];
            }

            for (var i = 0; i < _thumbCount; i++)
            {
                _thumbs[i].openPose = thumbs[i];
            }
        }

        public void SetClosedPose(HandPoseData data)
        {
            var fingers = data.RemapFingers(_fingerCount);
            var thumbs = data.RemapThumbs(_thumbCount);

            for (var i = 0; i < _fingerCount; i++)
            {
                Fingers[i].closedPose = fingers[i];
            }

            for (var i = 0; i < _thumbCount; i++)
            {
                _thumbs[i].closedPose = thumbs[i];
            }
        }

        public void SetBlendPose(HandPoseData data)
        {
            for (var i = 0; i < Fingers.Length; i++)
            {
                Fingers[i].blendPose = data.fingers[i];
            }

            for (var i = 0; i < _thumbs.Length; i++)
            {
                _thumbs[i].blendPose = data.thumbs[i];
            }
        }

#if UNITY_EDITOR
        public override void DrawGizmos()
        {
            base.DrawGizmos();

            using var color = TempGizmoColor.Create();

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(GetPointOnPalm(Vector2.up).position, 0.005f);

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
