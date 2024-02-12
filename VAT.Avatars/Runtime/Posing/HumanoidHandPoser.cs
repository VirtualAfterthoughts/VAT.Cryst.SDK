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

            _hand.Hand.Transform = descriptor.hand.Transform.Transform(offset);
            Solve();
        }

        public void Solve() {
            _hand.SetOpenPose(handPoseData);
            _hand.Solve();

            _artHand.SolveFingers();
        }

        public override void WriteArtOffsets()
        {
            Initiate();

            _hand.BindPose();
            _hand.Hand.Transform = descriptor.hand.Transform.Transform(offset);

            _artHand.WriteOffsets(_hand);

            _hand.NeutralPose();

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
