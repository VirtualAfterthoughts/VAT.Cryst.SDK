using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Mathematics;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Proportions;
using VAT.Avatars.Skeletal;
using VAT.Avatars.Vitals;
using VAT.Avatars.Art;
using VAT.Shared;
using VAT.Shared.Extensions;
using VAT.Avatars.Posing;
using VAT.Input;
using VAT.Input.Data;

namespace VAT.Avatars.Integumentary
{
    [RequireComponent(typeof(Animator))]
    [ExecuteAlways]
    public partial class HumanoidAvatar : Avatar {
        public Animator animator;

        public Transform eyeCenterOverride;

        public float density = 1.078f;

        public HumanoidProportions proportions;

        public HumanoidArtDescriptor artDescriptor;

        [HideInInspector]
        [Tooltip("The initial eye center position local to the avatar transform at runtime.")]
        public SerializedNullableVector3 runtimeEyeCenter = null;

        private Transform _physicsRoot;

        private HumanoidAvatarSkeleton _skeleton = null;
        public HumanoidAvatarSkeleton Skeleton => _skeleton;

        private HumanoidVitals _vitals = null;
        public HumanoidVitals Vitals => _vitals;

        public override IAvatarSkeleton GetSkeleton()
        {
            return _skeleton;
        }

        public override BodyMeasurements GetMeasurements()
        {
            return proportions.GetMeasurements();
        }

        public override IAvatarStats GetStats()
        {
            return Vitals.GetStats(proportions);
        }

#if UNITY_EDITOR
        public void Update() {
            if (Application.isPlaying)
                return;

            // Save eye center
            var editorEyeCenter = EditorGetEyeCenter();

            if (editorEyeCenter.HasValue) {
                var localEyeCenter = transform.InverseTransformPoint(editorEyeCenter.Value);

                if (!runtimeEyeCenter.HasValue() || !localEyeCenter.Approximately(runtimeEyeCenter.GetValueOrDefault())) {
                    runtimeEyeCenter = localEyeCenter;
                    EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssetIfDirty(this);
                }
            }
        }
#endif

        public override void WriteArtOffsets() {
            Skeleton.ArtSkeleton.WriteOffsets(Skeleton.DataSkeleton);
        }

        public override bool TryCreateHandPoser(out HandPoser poser) {
            if (!Initiated)
                Initiate();

            var humanPoser = gameObject.AddComponent<HumanoidHandPoser>();
            poser = humanPoser;

            humanPoser.proportions = proportions.rightArmProportions.handProportions;
            humanPoser.descriptor = artDescriptor.rightArmDescriptor.hand;
            humanPoser.offset = artDescriptor.rightArmDescriptor.hand.hand.Transform.InverseTransform(Skeleton.DataSkeleton.RightArm.Hand.Hand.Transform);

            humanPoser.WriteArtOffsets();

            return true;
        }

        protected override void OnInitiate() {
            _vitals = new HumanoidVitals();

            var dataSkeleton = new HumanoidDataSkeleton();
            var physSkeleton = new HumanoidPhysSkeleton();
            var artSkeleton = new HumanoidArtSkeleton();

            base.OnInitiate();

            // Initiate the data/IK skeleton
            dataSkeleton.Initiate();
            dataSkeleton.WriteProportions(proportions);
            dataSkeleton.BindPose();

            // Setup the internals of the physics skeleton, but don't actually create it yet
            physSkeleton.Initiate();

            // Now, initiate the art skeleton
            artSkeleton.Initiate();
            artSkeleton.WriteTransforms(artDescriptor);

            // Finally, save the skeletons
            _skeleton = new HumanoidAvatarSkeleton(dataSkeleton, physSkeleton, artSkeleton);
        }

        protected override void OnInitiateRuntime() {
            _physicsRoot = GameObjectExtensions.CreateGameObject(PhysSkeletonName, transform.parent).transform;

            base.OnInitiateRuntime();

            // Write eye center to the root transform
            var eyeCenter = Skeleton.DataSkeleton.Neck.EyeCenter;

            eyeCenter.rotation = transform.rotation;

            if (runtimeEyeCenter.HasValue()) {
                eyeCenter.position = transform.TransformPoint(runtimeEyeCenter.Value);
            }
            else {
                throw new MissingReferenceException("No HumanoidAvatar eye center was found at runtime! Please recompile your avatar!");
            }

            // Properly initiate the physics skeleton
            Skeleton.PhysSkeleton.InitiateRuntime();
            Skeleton.PhysSkeleton.WriteProportions(proportions);

            Skeleton.PhysSkeleton.SetTransformRoot(_physicsRoot);

            Skeleton.PhysSkeleton.WriteReferences(Skeleton.DataSkeleton);

            // Properly initiate the art skeleton
            Skeleton.ArtSkeleton.WriteData(Skeleton.PhysSkeleton);
            WriteArtOffsets();

            // Match physics skeleton to default pose
            Skeleton.DataSkeleton.NeutralPose();
            Skeleton.PhysSkeleton.MatchPose(Skeleton.DataSkeleton);

            // Initialize the vitals
            var payload = new HumanoidVitalsPayload();
            payload.InjectDependencies(proportions, Skeleton.PhysSkeleton);

            var vitalGroups = new HumanoidBoneGroupVitals[]
            {
                new HumanoidNeckVitals(),
                new HumanoidSpineVitals(),
                new HumanoidArmVitals(true),
                new HumanoidArmVitals(false),
                new HumanoidLegVitals(true),
                new HumanoidLegVitals(false),
            };

            Vitals.InjectDependencies(vitalGroups, payload);

            Vitals.CalculateVitals();
            Vitals.ApplyVitals();
        }

        protected override void OnUninitiateRuntime() {
            _physicsRoot.gameObject.SetActive(false);
            Destroy(_physicsRoot.gameObject);

            Skeleton.ArtSkeleton.Deinitiate();
        }
    }
}
