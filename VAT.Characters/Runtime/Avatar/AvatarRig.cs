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
    using VAT.Audio;
    using VAT.Packaging;
    using VAT.Avatars.Sounds;
    using VAT.Cryst.Math;

    [RequireRig(typeof(IBehaviourRig))]
    public class AvatarRig : CrystRig, IAvatarRig
    {
        public Avatar targetAvatar;

        public HandPose openPose;
        public HandPose closedPose;

        public AudioClip[] grabSounds;

        public ShardReferenceT<AudioCollection> footstepSounds;

        private Avatar _activeAvatar = null;

        public Avatar CurrentAvatar => _activeAvatar;

        public Action OnPostArt;

        List<IAvatarAbility> _constantAbilities = null;

        private AvatarSounds _avatarSounds = null;

        public override void OnRigEnable()
        {
            _constantAbilities = new List<IAvatarAbility>
            {
                new ForcePullAbility()
            };

            ChangeAvatar();

            RigManager.GetVitals().OnUpdatedVitals += OnUpdatedVitals;
        }

        private void InitiateAbilities()
        {
            foreach (var ability in _constantAbilities)
            {
                ability.OnInitiateAvatar(CurrentAvatar, this);
            }

            _avatarSounds = CurrentAvatar.GetComponent<AvatarSounds>();
        }

        private void DeinitiateAbilities()
        {
            foreach (var ability in _constantAbilities)
            {
                ability.OnDeinitiateAvatar(CurrentAvatar, this);
            }

            _avatarSounds = null;
        }

        public override void OnRigDisable()
        {
            if (_activeAvatar != null)
            {
                OnExitingAvatar?.Invoke(_activeAvatar);

                DeinitiateAbilities();

                _activeAvatar.Uninitiate();
                _activeAvatar = null;
            }

            RigManager.GetVitals().OnUpdatedVitals -= OnUpdatedVitals;
        }

        private void OnUpdatedVitals(ICrystVitals vitals)
        {
            ApplyRemapping();
        }

        public void ApplyRemapping()
        {
            var vitals = RigManager.GetVitals();

            if (vitals != null && _activeAvatar != null)
            {
                float scale = vitals.CharacterMeasurements.height / vitals.PlayerMeasurements.height;
                var playerMeasurements = BodyMeasurementHelper.Scale(vitals.PlayerMeasurements, scale);

                _activeAvatar.GetSkeleton().GetData().WriteRemappingMeasurements(playerMeasurements);
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

                OnExitingAvatar?.Invoke(_activeAvatar);

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

        public event Action<Avatar> OnSwitchedAvatar, OnExitingAvatar;

        public IInteractor[] GetCurrentInteractors()
        {
            return _interactors;
        }

        public void ActivateAvatar(Avatar avatar)
        {
            avatar.Initiate();

            var vitals = RigManager.GetVitals();
            vitals.CharacterMeasurements = avatar.GetMeasurements();
            vitals.UpdateVitals();

            var arms = avatar.GetArms();

            var behaviourRig = RigManager.GetRigOrNull<IBehaviourRig>();

            int index = 0;
            _interactors = new IInteractor[arms.Length];

            foreach (var arm in arms)
            {
                behaviourRig.TryGetArm(arm.Handedness, out var rigArm);
                var thing = rigArm.GetHand();

                // add interactor
                var bone = (PhysBone)arm.PhysArm.Hand.Hand;

                var interactor = bone.UnityGameObject.AddComponent<CrystInteractor>();
                interactor.controller = thing.GetInputController();
                interactor.hand = thing;
                interactor.arm = arm;
                interactor.handedness = arm.Handedness;
                interactor.grabSounds = grabSounds;

                interactor.openPose = openPose.data;
                interactor.closedPose = closedPose.data;

                _interactors[index++] = interactor;
            }

            var legs = avatar.GetLegs();

            foreach (var leg in legs)
            {
                leg.DataLeg.OnStep += OnStep;
            }

            _activeAvatar = avatar;

            ApplyRemapping();

            avatar.Write(GetPayload());
            avatar.GetSkeleton().GetData().Solve(1f);
            avatar.GetSkeleton().GetArt().Solve(1f);

            InitiateAbilities();

            OnSwitchedAvatar?.Invoke(avatar);
        }

        protected void OnStep(Vector3 position)
        {
            var footstepSounds = this.footstepSounds;

            if (_avatarSounds != null && _avatarSounds.FootstepLevels.Length > 0)
            {
                footstepSounds = _avatarSounds.FootstepLevels[0];
            }

            if (!footstepSounds.TryGetShard(out var shard))
            {
                return;
            }

            var clip = shard.GetRandomAudioClip();

            if (!clip.TryGetShard(out var clipShard))
            {
                return;
            }

            clipShard.MainAssetT.LoadAsset((c) =>
            {
                AudioSpawner.Spawn(new AudioSpawner.AudioRequestInfo()
                {
                    position = position,
                    settings = new AudioPlaySettings()
                    {
                        volume = UnityEngine.Random.Range(0.1f, 0.4f),
                        pitch = UnityEngine.Random.Range(0.7f, 1.1f),
                    },
                    clip = c,
                });
            });
        }

        protected virtual IAvatarPayload GetPayload()
        {
            _activeAvatar.transform.position = LastRig.transform.position;

            var root = SimpleTransform.Create(_activeAvatar.transform.position, _activeAvatar.transform.rotation);

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
            _activeAvatar.GetSkeleton().GetData().Solve(deltaTime);
            _activeAvatar.GetSkeleton().GetPhysics().Solve(deltaTime);
        }

        public override void OnLateUpdate(float deltaTime)
        {
            _activeAvatar.GetSkeleton().GetArt().Solve(deltaTime);

            ApplyOffsets();

            OnPostArt?.Invoke();
        }

        public override bool TryGetHead(out IInputJoint head)
        {
            head = new BasicJoint(SimpleTransform.Create(transform.position, transform.rotation).InverseTransform(_activeAvatar.GetSkeleton().GetPhysics().GetEyeCenter()));
            return true;
        }

        private void ApplyOffsets()
        {
            var behaviourRig = RigManager.GetRigOrNull<IBehaviourRig>();
            if (behaviourRig == null)
                return;

            // Rotation
            var skeleton = _activeAvatar.GetSkeleton();

            var root = behaviourRig.GetRoot();
            root.rotation = Quaternion.Slerp(root.rotation, skeleton.GetPhysics().GetRoot().Transform.rotation, Smoothing.CalculateDecay(24f, Time.deltaTime));

            // Position
            TryGetHead(out var thisHead);
            behaviourRig.TryGetHead(out var lastHead);

            var physHead = SimpleTransform.Create(transform.position, transform.rotation).Transform(thisHead.Transform);
            var head = root.Transform(lastHead.Transform);
             
            var pos = (physHead.position - head.position);

            root.position += pos;

            behaviourRig.SetRoot(root);
        }
    }
}
