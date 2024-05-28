using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Constants;
using VAT.Avatars.Bones;
using VAT.Input.Data;
using VAT.Shared.Data;
using VAT.Avatars.Proportions;
using Unity.Mathematics;

namespace VAT.Avatars.Muscular
{
    public class HumanoidPhysHand : HumanoidPhysBoneGroup, IHandGroup
    {
        public override int BoneCount => 1;

        public HumanoidPhysBone Hand => TBones[0];

        IBone IHandGroup.Hand => Hand;

        IBone IHandGroup.Palm => _relativePalm;

        private RelativeFinger[] _fingers;
        public IFingerGroup[] Fingers => _fingers;

        private RelativeThumb[] _thumbs;
        public IThumbGroup[] Thumbs => _thumbs;

        private RelativeBone _relativePalm;

        private IHandGroup _hand;

        public Transform fingerTransform;
        public MeshCollider fingerCollider;

        public HumanoidPhysHand()
        {
            Initiate();
        }

        public HumanoidPhysHand(PhysBoneGroup parent) : this()
        {
            Attach(parent);
        }

        public override void Initiate()
        {
            base.Initiate();

            _bones[0] = new HumanoidPhysBone($"Hand", null, HumanoidConstants.HandLimits);

            fingerTransform = new GameObject("Finger Collider").transform;
            fingerTransform.parent = Hand.UnityTransform;

            fingerCollider = fingerTransform.gameObject.AddComponent<MeshCollider>();
            fingerCollider.convex = true;
            Hand.InsertCollider(fingerCollider);
        }

        public void MatchFingers(IHandGroup hand)
        {
            _fingers = new RelativeFinger[hand.Fingers.Length];

            for (var i = 0; i < _fingers.Length; i++)
            {
                _fingers[i] = new RelativeFinger(Hand, hand.Hand, hand.Fingers[i]);
            }

            _thumbs = new RelativeThumb[hand.Thumbs.Length];

            for (var i = 0; i < _thumbs.Length; i++)
            {
                _thumbs[i] = new RelativeThumb(Hand, hand.Hand, hand.Thumbs[i]);
            }
        }

        public void MatchPose(IHandGroup hand)
        {
            _hand = hand;

            Hand.MatchBone(hand.Hand);

            _relativePalm = new RelativeBone(Hand, hand.Hand, hand.Palm);
        }

        public void WriteProportions(HumanoidArmProportions proportions)
        {
            var handMesh = GenerateHandMesh(proportions);
            var knuckleMesh = GenerateKnuckleMesh(proportions);

            Hand.SetMesh(handMesh);

            fingerTransform.localPosition = math.forward() * proportions.handProportions.wristEllipsoid.height;
            fingerCollider.sharedMesh = knuckleMesh;
        }

        public override void Solve()
        {
            var blendPose = GetBlendPose();

            if (blendPose.fingers == null)
            {
                return;
            }

            // Solve finger collider
            float averageCurl = 0f;
            int fingerCount = 0;

            foreach (var finger in blendPose.fingers)
            {
                averageCurl += finger.GetCurl();
                fingerCount++;
            }

            if (fingerCount > 0)
            {
                averageCurl /= fingerCount;
            }

            var openScale = new Vector3(1f, 0.4f, 0.85f);
            var closedScale = new Vector3(1f, 0.4f, 0f);

            var newScale = Vector3.Lerp(openScale, closedScale, averageCurl);
            var smoothScale = Vector3.Lerp(fingerTransform.localScale, newScale, Time.deltaTime * 12f);

            fingerTransform.localScale = smoothScale;
        }

        public SimpleTransform GetPointOnPalm(Vector2 position)
        {
            return _relativePalm.Transform.Transform(_hand.Palm.Transform.InverseTransform(_hand.GetPointOnPalm(position)));
        }

        public void SetOpenPose(HandPoseData data)
        {
            _hand.SetOpenPose(data);
        }

        public void SetClosedPose(HandPoseData data)
        {
            _hand.SetClosedPose(data);
        }

        public void SetBlendPose(HandPoseData data)
        {
            _hand.SetBlendPose(data);
        }

        public HandPoseData GetBlendPose()
        {
            return _hand.GetBlendPose();
        }

        public static Mesh GenerateHandMesh(HumanoidArmProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var wrist = proportions.handProportions.wristEllipsoid.Convert<Ellipse>();
            var knuckle = proportions.handProportions.knuckleEllipsoid.Convert<Ellipse>();

            // Create wrist -> knuckle
            quaternion rotation = Quaternion.AngleAxis(90f, math.right());

            EllipseCylinderMesh cylinder = new()
            {
                bottom = wrist,
                bottomTransform = SimpleTransform.Create(float3.zero, rotation),

                top = knuckle,
                topTransform = SimpleTransform.Create(math.forward() * proportions.handProportions.wristEllipsoid.height, rotation),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }

        public static Mesh GenerateKnuckleMesh(HumanoidArmProportions proportions)
        {
            // Convert ellipsoids to ellipses
            var knuckle = proportions.handProportions.knuckleEllipsoid.Convert<Ellipse>();

            // Create knuckle -> finger top
            quaternion rotation = Quaternion.AngleAxis(90f, math.right());

            EllipseCylinderMesh cylinder = new()
            {
                bottom = knuckle,
                bottomTransform = SimpleTransform.Create(float3.zero, rotation),

                top = knuckle,
                topTransform = SimpleTransform.Create(math.forward() * proportions.handProportions.knuckleEllipsoid.height, rotation),
            };

            // Create mesh
            return cylinder.CreateDescriptor().CreateMesh();
        }
    }
}
