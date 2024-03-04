using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using VAT.Avatars.Art;
using VAT.Avatars.Helpers;
using VAT.Avatars.Proportions;
using VAT.Avatars.Skeletal;
using VAT.Characters;
using VAT.Input;
using VAT.Interaction;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Avatars.Posing
{
    [ExecuteAlways]
    public sealed class HumanoidHandPoser : HandPoser {
        [HideInInspector]
        public SimpleTransform offset = SimpleTransform.Default;

        [HideInInspector]
        public HandProportions proportions;

        [HideInInspector]
        public HumanoidHandDescriptor descriptor;

        public HandPoseData handPoseData;

        public HandPose selectedPose;

        public Grip targetGrip;

        public HumanoidHand Hand => _hand;

        private HumanoidHand _hand;
        private HumanoidArtHand _artHand;

        protected override void OnInitiate() {
            base.OnInitiate();

            _hand = new HumanoidHand();
            _hand.Initiate();
            _hand.WriteProportions(proportions);
            _hand.BindPose();

            _artHand = new HumanoidArtHand();
            _artHand.Initiate();
            _artHand.WriteData(_hand);
            _artHand.WriteTransforms(descriptor);

            _hand.NeutralPose();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (_hand == null)
                return;

            Solve();
            SolveGrip();
        }

        public void ResetToNeutral()
        {
            handPoseData = new HandPoseData()
            {
                fingers = HandPoseCreator.CreateFingers(Hand.Fingers.Length),
                thumbs = HandPoseCreator.CreateThumbs(Hand.Thumbs.Length),
                centerOfPressure = Vector2.up,
                rotationOffset = Quaternion.identity,
            };
        }

        public void Solve() {
            _hand.Hand.Transform = descriptor.hand.Transform.Transform(offset);

            _hand.SetOpenPose(handPoseData);
            _hand.Solve();

            _artHand.SolveFingers();
        }

        public void SolveGrip()
        {
            if (targetGrip != null)
            {
                AvatarGrabberPoint grabberPoint = new AvatarGrabberPoint()
                {
                    hand = Hand,
                    radius = 0f,
                };

                var targetInHand = targetGrip.GetDefaultTargetInInteractor(grabberPoint, handPoseData);
                var targetInWorld = targetGrip.GetDefaultTargetInWorld(grabberPoint, handPoseData);

                transform.rotation = (targetInWorld.rotation * Quaternion.Inverse(grabberPoint.GetParentTransform().Transform(targetInHand).rotation) * transform.rotation);

                Solve();

                transform.position += (Vector3)(targetInWorld.position - grabberPoint.GetParentTransform().Transform(targetInHand).position);
            }
        }

        public override void WriteArtOffsets()
        {
            Initiate();

            _hand.BindPose();
            _hand.Hand.Transform = descriptor.hand.Transform.Transform(offset);

            _artHand.WriteOffsets(_hand);

            _hand.NeutralPose();

            ResetToNeutral();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR
        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            if (_hand == null)
                return;

            using (TempGizmoColor.Create()) {
                Gizmos.color = new Color(255, 10f, 0f, 255f) / 255f;

                _hand.DrawGizmos();
            }
        }
#endif
    }
}
