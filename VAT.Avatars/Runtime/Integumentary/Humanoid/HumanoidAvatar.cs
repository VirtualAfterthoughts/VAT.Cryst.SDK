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

namespace VAT.Avatars.Integumentary
{
    [RequireComponent(typeof(Animator))]
    [ExecuteAlways]
    public partial class HumanoidAvatar : AvatarT<HumanoidAvatarAnatomy> {
        public Animator animator;

        public Transform eyeCenterOverride;

        public float density = 1.078f;

        public HumanoidProportions proportions;

        public HumanoidArtDescriptor artDescriptor;

        [HideInInspector]
        [Tooltip("The initial eye center position local to the avatar transform at runtime.")]
        public SerializedNullableVector3 runtimeEyeCenter = null;

        [HideInInspector]
        [Tooltip("The height of the avatar in meters.")]
        public SerializedNullableFloat height = null;

        private Transform _physicsRoot;

        private HumanoidAvatarAnatomy _humanoidAvatarAnatomy;
        public override HumanoidAvatarAnatomy GenericAnatomy => _humanoidAvatarAnatomy;

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

            // Save height
            var height = EditorGetHeight();
            
            if (height.HasValue && (!this.height.HasValue() || !Mathf.Approximately(height.Value, this.height.GetValueOrDefault()))) {
                this.height = height.Value;
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }
        }
#endif

        public override void WriteArtOffsets() {
            GenericAnatomy.GenericSkeleton.GenericArtBoneSkeleton.WriteOffsets(GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton);
        }

        public override bool TryCreateHandPoser(out HandPoser poser) {
            if (!Initiated)
                Initiate();

            var humanPoser = gameObject.AddComponent<HumanoidHandPoser>();
            poser = humanPoser;

            humanPoser.proportions = proportions.rightArmProportions.handProportions;
            humanPoser.descriptor = artDescriptor.rightArmDescriptor.hand;
            humanPoser.offset = artDescriptor.rightArmDescriptor.hand.hand.Transform.InverseTransform(GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.RightArm.Hand.Hand.Transform);

            humanPoser.WriteArtOffsets();

            return true;
        }

        protected override void OnInitiate() {
            _humanoidAvatarAnatomy = new HumanoidAvatarAnatomy(
                new HumanoidAvatarSkeleton(new HumanoidDataSkeleton(), new HumanoidPhysSkeleton(), new HumanoidArtSkeleton()),
                new HumanoidVitals());

            base.OnInitiate();

            // Initiate the data/IK skeleton
            proportions.generalProportions = new HumanoidGeneralProportions() {
                height = height.GetValueOrDefault(),
            };

            GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.Initiate();
            GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.WriteProportions(proportions);
            GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.BindPose();

            // Setup the internals of the physics skeleton, but don't actually create it yet
            GenericAnatomy.GenericSkeleton.GenericPhysBoneSkeleton.Initiate();

            // Now, initiate the art skeleton
            GenericAnatomy.GenericSkeleton.GenericArtBoneSkeleton.Initiate();
            GenericAnatomy.GenericSkeleton.GenericArtBoneSkeleton.WriteTransforms(artDescriptor);
        }

        protected override void OnInitiateRuntime() {
            _physicsRoot = GameObjectExtensions.CreateGameObject(PhysSkeletonName, transform.parent).transform;

            base.OnInitiateRuntime();

            // Make sure the avatar has a height
            if (!height.HasValue())
                throw new MissingReferenceException("No HumanoidAvatar height was found at runtime! Please recompile your avatar!");

            // Write eye center to the root transform
            var eyeCenter = GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.Neck.EyeCenter;

            eyeCenter.rotation = transform.rotation;

            if (runtimeEyeCenter.HasValue()) {
                eyeCenter.position = transform.TransformPoint(runtimeEyeCenter.Value);
            }
            else {
                throw new MissingReferenceException("No HumanoidAvatar eye center was found at runtime! Please recompile your avatar!");
            }

            // Properly initiate the physics skeleton
            GenericAnatomy.GenericSkeleton.GenericPhysBoneSkeleton.InitiateRuntime();
            GenericAnatomy.GenericSkeleton.GenericPhysBoneSkeleton.WriteProportions(proportions);

            GenericAnatomy.GenericSkeleton.GenericPhysBoneSkeleton.SetTransformRoot(_physicsRoot);

            GenericAnatomy.GenericSkeleton.GenericPhysBoneSkeleton.WriteReferences(GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton);

            // Properly initiate the art skeleton
            GenericAnatomy.GenericSkeleton.GenericArtBoneSkeleton.WriteData(GenericAnatomy.GenericSkeleton.GenericPhysBoneSkeleton);
            WriteArtOffsets();

            // Match physics skeleton to default pose
            GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton.NeutralPose();
            GenericAnatomy.GenericSkeleton.GenericPhysBoneSkeleton.MatchPose(GenericAnatomy.GenericSkeleton.GenericDataBoneSkeleton);

            // Initialize the vitals
            var payload = new HumanoidVitalsPayload();
            payload.InjectDependencies(proportions, GenericAnatomy.GenericSkeleton.GenericPhysBoneSkeleton);

            var vitalGroups = new HumanoidBoneGroupVitals[]
            {
                new HumanoidNeckVitals(),
                new HumanoidSpineVitals(),
                new HumanoidArmVitals(true),
                new HumanoidArmVitals(false),
                new HumanoidLegVitals(true),
                new HumanoidLegVitals(false),
            };

            GenericAnatomy.GenericVitals.InjectDependencies(vitalGroups, payload);

            GenericAnatomy.GenericVitals.CalculateVitals();
            GenericAnatomy.GenericVitals.ApplyVitals();
        }

        protected override void OnUninitiateRuntime() {
            Destroy(_physicsRoot.gameObject);
        }

        protected override AvatarArm[] CreateArms() {
            var array = new AvatarArm[2];
            var skeleton = GenericAnatomy.GenericSkeleton;
            var dataSkeleton = skeleton.GenericDataBoneSkeleton;
            var physSkeleton = skeleton.GenericPhysBoneSkeleton;

            array[0] = new AvatarArm(Handedness.LEFT, dataSkeleton.Spine.Sacrum, physSkeleton.Spine.Sacrum, skeleton.GenericDataBoneSkeleton.LeftArm, skeleton.GenericPhysBoneSkeleton.LeftArm);
            array[1] = new AvatarArm(Handedness.RIGHT, dataSkeleton.Spine.Sacrum, physSkeleton.Spine.Sacrum, skeleton.GenericDataBoneSkeleton.RightArm, skeleton.GenericPhysBoneSkeleton.RightArm);

            return array;
        }
    }
}
