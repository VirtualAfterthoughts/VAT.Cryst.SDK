using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;
using UnityEngine;

using VAT.Avatars.Proportions;
using VAT.Avatars.Nervous;

using VAT.Shared.Data;
using VAT.Avatars.REWORK;

namespace VAT.Avatars.Skeletal
{
    public class HumanoidSpine : HumanoidBoneGroup, IHumanSpine
    {
        public DataBone T1Vertebra => Bones[0];
        public DataBone T7Vertebra => Bones[1];
        public DataBone L1Vertebra => Bones[2];
        public DataBone Sacrum => Bones[3];

        public DataBone Root = null;

        public override int BoneCount => 4;

        private HumanoidLocomotion _locomotion;
        public HumanoidLocomotion Locomotion => _locomotion;

        public AnimationCurve ThoraxTilt = new(new(0f, 0f, 1f, 1f), new(40f, 35f, 0.54f, 0.54f), new(100f, 45f));
        public AnimationCurve SacrumUpOffset = new(new(0f, 0f), new(0.5f, 1f), new(1f, 0f));

        public HumanoidNeck Neck => _neck;

        IBone IHumanSpine.Root => Root;

        IBone IHumanSpine.Sacrum => Sacrum;

        IBone IHumanSpine.L1Vertebra => L1Vertebra;

        IBone IHumanSpine.T7Vertebra => T7Vertebra;

        IBone IHumanSpine.T1Vertebra => T1Vertebra;

        private HumanoidNeck _neck;

        private HumanoidGeneralProportions _generalProportions;
        private HumanoidNeckProportions _neckProportions;
        private HumanoidSpineProportions _spineProportions;

        private quaternion lastChestRotation = quaternion.identity;

        public override void Initiate() {
            base.Initiate();

            Root = new DataBone();

            // Setup locomotion solving
            _locomotion = new HumanoidLocomotion();
            _locomotion.Initiate(Sacrum, L1Vertebra);
        }

        public override void Write(IAvatarPayload payload) {
            base.Write(payload);

            _locomotion.Write(payload);
        }

        public override void WriteProportions(HumanoidProportions proportions)
        {
            _generalProportions = proportions.generalProportions;
            _neckProportions = proportions.neckProportions;
            _spineProportions = proportions.spineProportions;

            _locomotion.WriteProportions(proportions);
        }

        public override void BindPose()
        {
            base.BindPose();

            T1Vertebra.localPosition = new(0f, -_neckProportions.lowerNeckEllipsoid.height, _spineProportions.upperChestOffsetZ);
            T7Vertebra.localPosition = new(0f, -_spineProportions.upperChestEllipsoid.height, _spineProportions.chestOffsetZ);
            L1Vertebra.localPosition = new(0f, -_spineProportions.chestEllipsoid.height, _spineProportions.spineOffsetZ);
            Sacrum.localPosition = new(0f, -_spineProportions.spineEllipsoid.height, _spineProportions.pelvisOffsetZ);
        }

        public override void Solve()
        {
            SimpleTransform root = _avatarPayload.GetRoot();

            Root.Transform = root;

            quaternion chestRotation = _neck.chestRotation;
            chestRotation = Quaternion.Slerp(root.TransformRotation(lastChestRotation), chestRotation, Time.deltaTime * 7f);
            lastChestRotation = root.InverseTransformRotation(chestRotation);

            T1Vertebra.rotation = chestRotation;

            Vector3 t1Up = T1Vertebra.up;
            Vector3 t1Right = T1Vertebra.right;

            float height = _generalProportions.height;

            float bendAngle = Vector3.Angle(root.up, t1Up);
            Vector3 bendAxis = Vector3.Cross(root.up, t1Up);
            var thoracicRotation = Quaternion.AngleAxis(-bendAngle, bendAxis);
            float neckHeight = Mathf.Clamp01(Vector3.Dot(root.up, T7Vertebra.position - root.position) / (height * 0.8f));
            float cervicalHeight = SacrumUpOffset.Evaluate(neckHeight);

            Vector3 vector = Quaternion.AngleAxis(cervicalHeight * 90f, thoracicRotation * t1Right) * root.up;
            float tiltAngle = Vector3.Angle(vector, t1Up);
            Vector3 tiltAxis = Vector3.Cross(vector, t1Up);
            Quaternion sacrumRotation = Quaternion.AngleAxis(-ThoraxTilt.Evaluate(tiltAngle), tiltAxis) * T1Vertebra.rotation;

            T7Vertebra.rotation = Quaternion.Lerp(T1Vertebra.rotation, sacrumRotation, 0.3f);
            L1Vertebra.rotation = Quaternion.Lerp(T1Vertebra.rotation, sacrumRotation, 0.5f);
            Sacrum.rotation = sacrumRotation;

            // Solve locomotion logic for the legs
            _locomotion.Solve();
        }
        public override void Attach(DataBoneGroup group) {
            base.Attach(group);

            _neck = group as HumanoidNeck;
        }
    }
}
