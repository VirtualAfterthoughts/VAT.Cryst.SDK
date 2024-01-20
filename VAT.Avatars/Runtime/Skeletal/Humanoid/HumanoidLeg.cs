using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Proportions;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

using static Unity.Mathematics.math;

namespace VAT.Avatars.Skeletal
{
    using Unity.Mathematics;
    using VAT.Avatars.REWORK;

    public class HumanoidLeg : HumanoidBoneGroup, IHumanLeg
    {
        public bool isLeft = false;

        public DataBone Hip => Bones[0];
        public DataBone Knee => Bones[1];
        public DataBone Ankle => Bones[2];
        public DataBone Toe => Bones[3];

        public override int BoneCount => 4;

        IBone IHumanLeg.Hip => Hip;

        IBone IHumanLeg.Knee => Knee;

        IBone IHumanLeg.Ankle => Ankle;

        private int _legIndex;

        private HumanoidSpine _spine;

        private float _hipOffset;

        private HumanoidSpineProportions _spineProportions;
        private HumanoidLegProportions _legProportions;

        public override void WriteProportions(HumanoidProportions proportions) {
            _spineProportions = proportions.spineProportions;

            if (isLeft) {
                _legProportions = proportions.leftLegProportions;
                _legIndex = 0;
            }
            else {
                _legProportions = proportions.rightLegProportions;
                _legIndex = 1;
            }
        }

        public override void BindPose()
        {
            base.BindPose();

            float mult = isLeft ? -1f : 1f;

            _hipOffset = mult * (_legProportions.hipSeparationOffset + _legProportions.hipEllipsoid.radius.x);

            Hip.localPosition = new float3(_hipOffset, -_spineProportions.pelvisEllipsoid.height * 0.24f, 0f);
            Knee.localPosition = new float3(0f, -_legProportions.hipEllipsoid.height, -_legProportions.kneeOffsetZ);
            Ankle.localPosition = new float3(0f, -_legProportions.kneeEllipsoid.height, -_legProportions.ankleOffsetZ);
            Toe.localPosition = new float3(mult * _legProportions.toeOffset.x, -_legProportions.ankleEllipsoid.height + _legProportions.toeOffset.y, _legProportions.toeOffset.z);
        }

        public override void Solve()
        {
            // Get our locomotor and its solved heel position
            var locomotor = _spine.Locomotion.Locomotors[_legIndex];

            // Then, simply solve with that as the target
            LegSolve(locomotor.Result);
        }

        private void LegSolve(SimpleTransform target) {
            var position = target.position;
            var rotation = target.rotation;

            Vector3 legVector = position - Hip.position - mul(rotation, Vector3.up) * Toe.localPosition.y;
            float a = legVector.magnitude;
            float b = _legProportions.hipEllipsoid.height;
            float c = _legProportions.kneeEllipsoid.height;

            float A = Mathf.Acos(((Mathf.Pow(a, 2f) + Mathf.Pow(b, 2f) - Mathf.Pow(c, 2f)) / (2f * a * b)).SinClamp()) * Mathf.Rad2Deg;
            float B = Mathf.Acos(((Mathf.Pow(b, 2f) + Mathf.Pow(c, 2f) - Mathf.Pow(a, 2f)) / (2f * b * c)).SinClamp()) * Mathf.Rad2Deg;

            // Get right bend direction
            Vector3 footRht = mul(rotation, Vector3.right);

            Vector3 bendTwist = Quaternion.AngleAxis(90f, -footRht) * Vector3.ProjectOnPlane(-legVector, footRht);
            Hip.rotation = Quaternion.LookRotation(legVector, bendTwist);

            // Apply cosine
            Hip.rotation = Quaternion.AngleAxis(-A, -Hip.right) * Hip.rotation;
            var kneeRotation = Quaternion.AngleAxis(180f - B, -Hip.right) * Hip.rotation;
            Hip.rotation = Quaternion.LookRotation(-Hip.up, -Hip.forward);

            // The rest is already solved, we just apply it
            Knee.rotation = kneeRotation;
            Knee.rotation = Quaternion.LookRotation(-Knee.up, -Knee.forward);

            Ankle.rotation = rotation;

            // Foot ball pass
            float heelOffset = 0f;
            Ankle.rotation = Quaternion.AngleAxis(Mathf.Clamp(90f - Vector3.Angle(position - Ankle.position, (position - mul(rotation, Vector3.up) * heelOffset) - Toe.position), 0f, 45f), Ankle.right) * Ankle.rotation;

            Toe.rotation = rotation;
        }

        public float3 GetFootCenter() {
            float3 forward = Ankle.forward;
            float3 up = Ankle.up;
            
            float3 heel = Ankle.position - up * _legProportions.ankleEllipsoid.height;

            heel += ((-forward * _legProportions.ankleEllipsoid.radius.y) + (forward * Toe.localPosition.z)) * 0.5f;

            return heel;
        }

        public override void Attach(DataBoneGroup group)
        {
            base.Attach(group);

            _spine = group as HumanoidSpine;
        }
    }
}
