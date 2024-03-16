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
    using VAT.Interaction;
    using VAT.Shared.Data;

    public class AvatarRig : CrystRig
    {
        public Avatar avatar;

        public HandPose openPose;
        public HandPose closedPose;

        public override void OnAwake()
        {
            avatar.Initiate();

            var arms = avatar.GetArms();

            TryGetTrackedRig(out var rig);

            foreach (var arm in arms)
            {
                rig.TryGetArm(arm.Handedness, out var rigArm);
                rigArm.TryGetHand(out var thing);

                // add interactor
                var bone = (PhysBone)arm.PhysArm.Hand.Hand;

                var interactor = bone.UnityGameObject.AddComponent<CrystInteractor>();
                interactor.controller = thing.GetInputControllerOrDefault();
                interactor.hand = thing.GetInputHandOrDefault();
                interactor.arm = arm;
                interactor.handedness = arm.Handedness;

                interactor.openPose = openPose.data;
                interactor.closedPose = closedPose.data;

                foreach (var physBone in arm.PhysArm.Bones)
                {
                    interactor.hosts.Add(((PhysBone)physBone).UnityGameObject.AddComponent<InteractableHost>());
                }
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
                Head = head.Transform,
                LeftArm = leftArm,
                RightArm = rightArm,
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
            head = new BasicJoint(SimpleTransform.Create(transform).InverseTransform(avatar.Anatomy.Skeleton.PhysBoneSkeleton.GetEyeCenter()));
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
