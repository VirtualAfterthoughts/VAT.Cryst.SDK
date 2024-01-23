using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.XR;
using VAT.Avatars.Data;
using VAT.Avatars.Proportions;

using VAT.Shared.Data;
using VAT.Shared.Utilities;

namespace VAT.Avatars.Skeletal
{
    public class HumanoidFinger : DataBoneGroup
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

        public SimpleTransform defaultEnd;

        public Vector3 forward;
        public Vector3 right;
        public Vector3 up;

        public quaternion defaultRotation = quaternion.identity;

        public SimpleTransform openPoint;

        public SimpleTransform closedPoint;

        public float curl;

        public override void BindPose() {
            base.BindPose();

            MetaCarpal.localPosition = _proportions.metaCarpalTransform.position;
            MetaCarpal.localRotation = _proportions.metaCarpalTransform.rotation;

            Proximal.localPosition = _proportions.proximalTransform.position;
            Proximal.localRotation = _proportions.proximalTransform.rotation;

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

            var handTransform = _hand.Transform;
            forward = handTransform.InverseTransformDirection(MetaCarpal.forward);
            right = handTransform.InverseTransformDirection(MetaCarpal.right);
            up = handTransform.InverseTransformDirection(MetaCarpal.up);

            defaultEnd = handTransform.InverseTransform(End.Transform);

            openPoint = SimpleTransform.Default;
            closedPoint = openPoint;

            for (var i = 0; i < _boneCount; i++)
            {
                var rotation = _bones[i].localRotation;
                openRotations[i] = rotation;
                closedRotations[i] = rotation;
            }
        }

        public FingerPose GetPose() {
            return new FingerPose()
            {
                point = NormalizeTarget()
            };
        }

        public SimpleTransform NormalizeTarget() {
            var wrist = _handProportions.wristEllipsoid;
            var normalized = NeutralEndBone.InverseTransform(End.Transform);
            var fingerLength = _proportions.GetLength();
            normalized.position /= new float3(wrist.radius.x, wrist.radius.y, fingerLength);
            return normalized;
        }

        public void SetTarget(SimpleTransform normalized) {
            var wrist = _handProportions.wristEllipsoid;
            var fingerLength = _proportions.GetLength();
            normalized.position *= new float3(wrist.radius.x, wrist.radius.y, fingerLength);
            openPoint = normalized;
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

            openRotations = new Quaternion[_boneCount];
            closedRotations = new Quaternion[_boneCount];
        }

        private Quaternion[] openRotations;
        private Quaternion[] closedRotations;

        public override void Solve()
        {
            var target = NeutralEndBone.Transform(this.openPoint);
            var right = MetaCarpal.Parent.Transform.TransformDirection(this.right);
            var up = MetaCarpal.Parent.Transform.TransformDirection(this.up);

            for (var i = 0; i < BoneCount; i++) {
                Bones[i].localRotation = openRotations[i];
            }

            SolveBones(target, right, up);

            for (var i = 0; i < BoneCount; i++) {
                openRotations[i] = Bones[i].localRotation;
                Bones[i].localRotation = closedRotations[i];
            }

            target = NeutralEndBone.Transform(this.closedPoint);

            SolveBones(target, right, up);

            for (var i = 0; i < BoneCount; i++) {
                closedRotations[i] = Bones[i].localRotation;
            }

            for (var i = 0; i < BoneCount; i++) {
                Bones[i].localRotation = Quaternion.Lerp(openRotations[i], closedRotations[i], curl);
            }
        }

        private void SolveBones(SimpleTransform target, float3 right, float3 up) {
            target.position -= target.forward * _distalLength;

            for (var r = 0; r < 1; r++)
            {
                int number = 2;

                // Iterate from tip to base
                for (var i = BoneCount - (number + 1); i >= 0; --i)
                {
                    var end = Bones[BoneCount - number];
                    var bone = Bones[i];

                    Quaternion rotation_i = Quaternion.FromToRotation(end.position - bone.position, target.position - bone.position);

                    bone.rotation = rotation_i * bone.rotation;

                    if (i != 0)
                    {
                        Quaternion angleClamp = Quaternion.FromToRotation(bone.right, bone.Parent.right);
                        bone.rotation = angleClamp * bone.rotation;
                    }

                    if (i >= 1)
                    {
                        Vector3 axis = bone.Parent.right;
                        float angle = Vector3.SignedAngle(bone.Parent.forward, bone.forward, axis);

                        if (i == BoneCount - 2)
                            angle = Mathf.Clamp(angle, -30f, 160f);
                        else
                            angle = Mathf.Clamp(angle, 0f, 160f);

                        var newForward = Quaternion.AngleAxis(angle, axis) * bone.Parent.forward;

                        bone.rotation = Quaternion.FromToRotation(bone.forward, newForward) * bone.rotation;
                    }

                    if (i == 0)
                    {
                        Vector3 axis = up;
                        float angle = Vector3.SignedAngle(right, bone.right, axis);

                        angle = Mathf.Clamp(angle, -20f, 70f);

                        var newRight = Quaternion.AngleAxis(angle, axis) * right;

                        bone.rotation = Quaternion.FromToRotation(bone.right, newRight) * bone.rotation;
                    }
                }
            }

            Distal.rotation = target.rotation;
        }
    }
}
