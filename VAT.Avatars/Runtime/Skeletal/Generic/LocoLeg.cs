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

        private Vector3 _trackedDebt;

        private Vector3 _localHead;

        public override void BindPose() {
            base.BindPose();

            Knee.localPosition = float3.zero;
            Foot.localPosition = down() * _length;
        }

        public void WriteProportions(float length) {
            _length = length;
        }

        public float _spineDebtMultiplier = 1f;
        public float _footShrink = 0f;
        public float _jumpPull = 0f;
        public float _jumpMultiplier = 1f;
        private float _timeSinceJump = 0f;

        public override void Solve()
        {
            SimpleTransform root = _avatarPayload.GetRoot();

            _avatarPayload.TryGetHead(out SimpleTransform head);

            _trackedDebt += (Vector3)PhysicsExtensions.GetLinearVelocity(root.TransformPoint(_localHead), head.position);
            _localHead = root.InverseTransformPoint(head.position);

            var parent = Knee.Parent;
            var forward = parent.forward;
            var up = root.up;
            var flattened = ((Vector3)forward).FlattenNeck(parent.up, up);

            Knee.rotation = Quaternion.LookRotation(flattened, up);

            var debt = _trackedDebt * 0.25f;
            velocity = debt;
            _trackedDebt -= debt;

            float distanceToFloor = root.InverseTransformPoint(Knee.position).y;
            float extension = 0f;

            if (_avatarPayload.TryGetInput(out var input)) {
                var movement = input.GetMovement();
                velocity += movement * 4f;

                if (input.GetJump())
                {
                    _jumpPull = Mathf.Lerp(_jumpPull, 0.4f, Time.deltaTime * 8f);

                    _jumpMultiplier = 1f;
                    _timeSinceJump = 0f;
                    _footShrink = 0f;
                    _spineDebtMultiplier = 1f;
                }
                else
                {
                    if (_timeSinceJump < 0.25f)
                    {
                        _jumpMultiplier = Mathf.Lerp(0f, 5f, _timeSinceJump / 0.25f);
                        _spineDebtMultiplier = Mathf.Lerp(0f, 1f, _timeSinceJump / 0.25f);
                        _jumpPull = Mathf.Lerp(0.4f, -0.5f, _timeSinceJump / 0.1f);
                    }
                    else
                    {
                        _jumpMultiplier = 1f;
                        _spineDebtMultiplier = 1f;
                        _jumpPull = Mathf.Lerp(_jumpPull, 0f, Time.deltaTime * 6f);
                    }

                    if (_timeSinceJump > 0.15f && _timeSinceJump < 1f)
                    {
                        _footShrink = 0.3f;
                    }
                    else
                    {
                        _footShrink = 0f;
                    }

                    _timeSinceJump += Time.deltaTime;
                }

                float offset = _jumpPull * _length;
                distanceToFloor -= offset;
                extension = Mathf.Abs(offset);
            }

            Foot.localPosition = down() * Mathf.Clamp(distanceToFloor, 0f, _length + extension);
        }

#if UNITY_EDITOR
        // No reason to draw gizmos for this
        public override void DrawGizmos() { }
#endif
    }
}
