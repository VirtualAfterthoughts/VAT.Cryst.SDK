using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Characters
{
    using VAT.Avatars;
    using VAT.Avatars.Integumentary;
    using VAT.Avatars.Muscular;
    using VAT.Avatars.Nervous;

    using VAT.Input.Skeleton;
    using VAT.Input;
    using VAT.Interaction;
    using VAT.Shared.Data;
    using VAT.Input.Data;

    using System;

    [RequireRig(typeof(IBehaviourRig))]
    public class AvatarRig : CrystRig, IAvatarRig
    {
        public Avatar targetAvatar;

        public HandPose openPose;
        public HandPose closedPose;

        public AudioClip[] grabSounds;

        private Avatar _activeAvatar = null;

        public Avatar CurrentAvatar => _activeAvatar;

        public Action OnPostArt;

        List<IAvatarAbility> _constantAbilities = null;

        public override void OnRigEnable()
        {
            _constantAbilities = new List<IAvatarAbility>
            {
                new ForcePullAbility()
            };

            ChangeAvatar();

            RigManager.GetVitalsOrNull().OnUpdatedVitals += OnUpdatedVitals;
        }

        private void InitiateAbilities()
        {
            foreach (var ability in _constantAbilities)
            {
                ability.OnInitiateAvatar(CurrentAvatar, this);
            }
        }

        private void DeinitiateAbilities()
        {
            foreach (var ability in _constantAbilities)
            {
                ability.OnDeinitiateAvatar(CurrentAvatar, this);
            }
        }

        public override void OnRigDisable()
        {
            if (_activeAvatar != null)
            {
                DeinitiateAbilities();

                _activeAvatar.Uninitiate();
                _activeAvatar = null;
            }

            RigManager.GetVitalsOrNull().OnUpdatedVitals -= OnUpdatedVitals;
        }

        private void OnUpdatedVitals(ICrystVitals vitals)
        {
            ApplyRemapping();
        }

        public void ApplyRemapping()
        {
            var vitals = RigManager.GetVitalsOrNull();

            if (vitals != null && _activeAvatar != null)
            {
                float scale = vitals.CharacterMeasurements.height / vitals.PlayerMeasurements.height;
                var playerMeasurements = BodyMeasurementHelper.Scale(vitals.PlayerMeasurements, scale);

                _activeAvatar.Anatomy.Skeleton.DataBoneSkeleton.WriteRemappingMeasurements(playerMeasurements);
            }
        }

        public void SwitchAvatar(Avatar avatar)
        {
            targetAvatar = avatar;
            ChangeAvatar();
        }

        [ContextMenu("Change Avatar")]
        public void ChangeAvatar()
        {
            if (_activeAvatar != null)
            {
                targetAvatar.transform.SetPositionAndRotation(_activeAvatar.transform.position, _activeAvatar.transform.rotation);

                DeinitiateAbilities();

                _activeAvatar.Uninitiate();
                _activeAvatar.gameObject.SetActive(false);

                _activeAvatar = null;

                _interactors = new IInteractor[0];
            }

            targetAvatar.gameObject.SetActive(true);

            ActivateAvatar(targetAvatar);
        }

        private IInteractor[] _interactors = new IInteractor[0];

        public IInteractor[] GetCurrentInteractors()
        {
            return _interactors;
        }

        public void ActivateAvatar(Avatar avatar)
        {
            avatar.Initiate();

            var vitals = RigManager.GetVitalsOrNull();
            vitals.CharacterMeasurements = avatar.GetMeasurements();
            vitals.UpdateVitals();

            var arms = avatar.GetArms();

            var behaviourRig = RigManager.GetRigOrNull<IBehaviourRig>();

            int index = 0;
            _interactors = new IInteractor[arms.Length];

            foreach (var arm in arms)
            {
                behaviourRig.TryGetArm(arm.Handedness, out var rigArm);
                var thing = rigArm.GetHandOrNull();

                // add interactor
                var bone = (PhysBone)arm.PhysArm.Hand.Hand;

                var interactor = bone.UnityGameObject.AddComponent<CrystInteractor>();
                interactor.controller = thing.GetInputControllerOrNull();
                interactor.hand = thing;
                interactor.arm = arm;
                interactor.handedness = arm.Handedness;
                interactor.grabSounds = grabSounds;

                interactor.openPose = openPose.data;
                interactor.closedPose = closedPose.data;

                foreach (var physBone in arm.PhysArm.Bones)
                {
                    interactor.hosts.Add(((PhysBone)physBone).UnityGameObject.AddComponent<InteractableHost>());
                }

                _interactors[index++] = interactor;
            }

            _activeAvatar = avatar;

            ApplyRemapping();

            avatar.Write(GetPayload());
            avatar.Anatomy.Skeleton.DataBoneSkeleton.Solve();
            avatar.SolveArt();

            InitiateAbilities();
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

            OnPostArt?.Invoke();
        }

        public override bool TryGetHead(out IJoint head)
        {
            head = new BasicJoint(SimpleTransform.Create(transform).InverseTransform(_activeAvatar.Anatomy.Skeleton.PhysBoneSkeleton.GetEyeCenter()));
            return true;
        }

        private void ApplyOffsets()
        {
            var behaviourRig = RigManager.GetRigOrNull<IBehaviourRig>();
            if (behaviourRig == null)
                return;

            // Rotation
            var skeleton = _activeAvatar.Anatomy.Skeleton;

            var root = behaviourRig.GetRoot();
            root.rotation = Quaternion.Slerp(root.rotation, skeleton.PhysBoneSkeleton.GetRoot().Transform.rotation, Time.deltaTime * 12f);

            // Position
            TryGetHead(out var thisHead);
            behaviourRig.TryGetHead(out var lastHead);

            var physHead = SimpleTransform.Create(transform).Transform(thisHead.Transform);
            var head = root.Transform(lastHead.Transform);
             
            var pos = (physHead.position - head.position);

            root.position += pos;

            behaviourRig.SetRoot(root);
        }
    }
}
