using System.Collections;
using System.Collections.Generic;

using static Unity.Mathematics.math;

using UnityEngine;

namespace VAT.Avatars.Skeletal
{
    using Unity.Mathematics;
    using VAT.Shared.Data;
    using VAT.Shared.Extensions;

    public class LocoLeg : DataBoneGroup
    {
        public override int BoneCount => 2;

        public DataBone Knee => Bones[0];
        public DataBone Foot => Bones[1];

        private float _length = 0f;
        public float Length => _length;

        public Vector3 velocity;

        private Vector3 _localHead;

        public override void BindPose() {
            base.BindPose();

            Knee.localPosition = float3.zero;
            Foot.localPosition = down() * _length;
        }

        public void WriteProportions(float length) {
            _length = length;
        }

        public override void Solve()
        {
            SimpleTransform root = _avatarPayload.GetRoot();

            _avatarPayload.TryGetHead(out SimpleTransform head);

            velocity = PhysicsExtensions.GetLinearVelocity(root.TransformPoint(_localHead), head.position);
            _localHead = root.InverseTransformPoint(head.position);

            var parent = Knee.Parent;
            var forward = parent.forward;
            var up = root.up;
            var flattened = ((Vector3)forward).FlattenNeck(parent.up, up);

            Knee.rotation = Quaternion.LookRotation(flattened, up);

            float distanceToFloor = root.InverseTransformPoint(Knee.position).y;
            Foot.localPosition = down() * distanceToFloor;

            if (_avatarPayload.TryGetInput(out var input)) {
                var movement = input.GetMovement();
                velocity += (Vector3)mul(Knee.rotation, new Vector3(movement.x, 0f, movement.y)) * 4f;
            }
        }

#if UNITY_EDITOR
        // No reason to draw gizmos for this
        public override void DrawGizmos() { }
#endif
    }
}
