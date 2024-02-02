using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.XR;
using VAT.Avatars.Proportions;
using VAT.Avatars.REWORK;
using VAT.Shared.Data;
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

        public Vector3 forward;
        public Vector3 right;
        public Vector3 up;

        public quaternion defaultRotation = quaternion.identity;

        public SimpleTransform openPoint;

        public SimpleTransform closedPoint;

        public bool isThumb;

        public float curl;

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

        public float splay;
        public float stretched;
        public float spread;
        public float twist;
        public float curl01 = 0.2f;
        public float curl02 = 0.2f;
        public float curl03 = 0.2f;

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

        public override void Solve()
        {
            Vector3 fwd = MetaCarpal.Parent.forward;
            Vector3 handUp = MetaCarpal.Parent.up;

            if (isThumb)
            {
                MetaCarpal.localRotation = Quaternion.AngleAxis(90f, Vector3.forward);
                MetaCarpal.rotation = Quaternion.AngleAxis(Mathf.Lerp(0f, -60f, Remap(stretched)), MetaCarpal.up) * MetaCarpal.rotation;
                MetaCarpal.rotation = Quaternion.AngleAxis(Mathf.Lerp(10f, -60f, Remap(spread)), MetaCarpal.right) * MetaCarpal.rotation;
                MetaCarpal.rotation = Quaternion.AngleAxis(Mathf.Lerp(-20f, 60f, Remap(twist)), MetaCarpal.forward) * MetaCarpal.rotation;
                Proximal.localRotation = Quaternion.identity;
                Middle.localRotation = Quaternion.AngleAxis(Mathf.Lerp(-90f, 90f, Remap(curl02)), Vector3.right);
                Distal.localRotation = Quaternion.AngleAxis(Mathf.Lerp(-90f, 90f, Remap(curl03)), Vector3.right);
                return;
            }

            //Proximal.localRotation = Quaternion.AngleAxis(90f, Vector3.right);
            //Middle.localRotation = Quaternion.AngleAxis(90f, Vector3.right);
            //Distal.localRotation = Quaternion.AngleAxis(90f, Vector3.right);

            //Vector3 fwd = Proximal.position - MetaCarpal.Parent.position;
            //Vector3 handUp = MetaCarpal.Parent.up;
            //Vector3.OrthoNormalize(ref handUp, ref fwd);

            Proximal.rotation = Quaternion.LookRotation(fwd, handUp);
            // Splay
            Proximal.rotation = Quaternion.AngleAxis(-GetSplayAngle(splay), handUp) * Proximal.rotation;
            
            // Curl 01
            Proximal.rotation = Quaternion.AngleAxis(GetCurlAngle(curl01), Proximal.right) * Proximal.rotation;
            
            // Curl 02
            Middle.localRotation = Quaternion.AngleAxis(GetCurlAngle(curl02), Vector3.right);
            
            // Curl 03
            Distal.localRotation = Quaternion.AngleAxis(GetCurlAngle(curl03), Vector3.right);

            return;

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
