using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Characters
{
    using VAT.Avatars;
    using VAT.Avatars.Integumentary;
    using VAT.Avatars.Muscular;
    using VAT.Avatars.Nervous;

    using VAT.Input;
    using VAT.Interaction;
    using VAT.Shared.Data;

    public class AvatarRig : CrystRig
    {
        public Avatar targetAvatar;

        public HandPose openPose;
        public HandPose closedPose;

        private Avatar _activeAvatar = null;

        public override void OnRigEnable()
        {
            ChangeAvatar();
        }

        public override void OnRigDisable()
        {
            if (_activeAvatar != null)
            {
                _activeAvatar.Uninitiate();
                _activeAvatar = null;
            }
        }

        [ContextMenu("Change Avatar")]
        public void ChangeAvatar()
        {
            if (_activeAvatar != null)
            {
                targetAvatar.transform.SetPositionAndRotation(_activeAvatar.transform.position, _activeAvatar.transform.rotation);

                _activeAvatar.Uninitiate();
                _activeAvatar.gameObject.SetActive(false);

                _activeAvatar = null;
            }

            targetAvatar.gameObject.SetActive(true);

            ActivateAvatar(targetAvatar);
        }

        public void ActivateAvatar(Avatar avatar)
        {
            avatar.Initiate();

            var vitals = RigManager.GetVitalsOrDefault();
            vitals.CharacterMeasurements = avatar.GetMeasurements();
            vitals.UpdateVitals();

            var arms = avatar.GetArms();

            TryGetTrackedRig(out var rig);

            foreach (var arm in arms)
            {
                rig.TryGetArm(arm.Handedness, out var rigArm);
                var thing = rigArm.GetHandOrNull();

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

            _activeAvatar = avatar;
        }

        protected virtual IAvatarPayload GetPayload()
        {
            _activeAvatar.transform.position = LastRig.transform.position;

            var root = SimpleTransform.Create(_activeAvatar.transform);
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

            _activeAvatar.Write(payload);
            _activeAvatar.Anatomy.Skeleton.DataBoneSkeleton.Solve();
            _activeAvatar.Anatomy.Skeleton.PhysBoneSkeleton.Solve();
        }

        public override void OnLateUpdate(float deltaTime)
        {
            _activeAvatar.SolveArt();

            ApplyOffsets();
        }

        public override bool TryGetHead(out IJoint head)
        {
            head = new BasicJoint(SimpleTransform.Create(transform).InverseTransform(_activeAvatar.Anatomy.Skeleton.PhysBoneSkeleton.GetEyeCenter()));
            return true;
        }

        private void ApplyOffsets()
        {
            if (!TryGetTrackedRig(out var trackedRig))
                return;

            // Rotation
            var skeleton = _activeAvatar.Anatomy.Skeleton;
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
