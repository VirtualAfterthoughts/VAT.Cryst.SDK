using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Characters
{
    using VAT.Avatars;
    using VAT.Avatars.Example;
    using VAT.Avatars.Integumentary;
    using VAT.Avatars.Muscular;
    using VAT.Avatars.Nervous;
    using VAT.Input;
    using VAT.Shared.Data;

    public class AvatarRig : CrystRig
    {
        public Avatar avatar;

        public override void OnAwake()
        {
            avatar.Initiate();

            var arms = avatar.GetArms();

            TryGetTrackedRig(out var rig);

            foreach (var arm in arms)
            {
                rig.TryGetArm(arm.Handedness, out var rigArm);
                rigArm.TryGetHand(out var thing);
                thing.TryGetInputController(out var controller);

                var go = ((PhysBone)arm.PhysArm.Hand.Hand).UnityGameObject;
                var interactor = go.AddComponent<CrystInteractor>();
                interactor.handedness = arm.Handedness;
                interactor.controller = controller;
                interactor.arm = arm;
            }
        }

        protected virtual IAvatarPayload GetPayload()
        {
            avatar.transform.position = LastRig.transform.position;

            var root = SimpleTransform.Create(avatar.transform);
            root.lossyScale = Vector3.one;

            LastRig.TryGetHead(out var head);
            LastRig.TryGetArm(Handedness.LEFT, out var leftArm);
            LastRig.TryGetArm(Handedness.RIGHT, out var rightArm);

            TryGetInput(out var input);

            return new BasicAvatarPayload()
            {
                Root = root,
                Head = root.Transform(head.Transform),
                LeftArm = root.TransformLimb(leftArm),
                RightArm = root.TransformLimb(rightArm),
                Input = input,
            };
        }

        public override void OnFixedUpdate(float deltaTime)
        {
            ApplyOffsets();

            var payload = GetPayload();

            avatar.Write(payload);
            avatar.Anatomy.Skeleton.DataBoneSkeleton.Solve();
            avatar.Anatomy.Skeleton.PhysBoneSkeleton.Solve();
        }

        public override void OnLateUpdate(float deltaTime)
        {
            avatar.SolveArt();

            ApplyOffsets();
        }

        public override bool TryGetHead(out IJoint head)
        {
            head = new BasicJoint(SimpleTransform.Create(transform).InverseTransform(avatar.Anatomy.Skeleton.PhysBoneSkeleton.GetHead()));
            return true;
        }

        private void ApplyOffsets()
        {
            if (!TryGetTrackedRig(out var trackedRig))
                return;

            // Rotation
            var skeleton = avatar.Anatomy.Skeleton;
            trackedRig.transform.rotation = Quaternion.Slerp(trackedRig.transform.rotation, skeleton.PhysBoneSkeleton.GetRoot().Transform.rotation, Time.deltaTime * 12f);

            // Position
            TryGetHead(out var thisHead);
            trackedRig.TryGetHead(out var lastHead);

            var physHead = SimpleTransform.Create(transform).Transform(thisHead.Transform);
            var head = SimpleTransform.Create(trackedRig.transform).Transform(lastHead.Transform);
             
            var pos = (physHead.position - head.position);
             
            trackedRig.transform.position += (Vector3)pos;
        }
    }
}
