using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Art
{
    public interface IHumanoidDescriptor {
        public void AutoFillBones(Animator animator);
    }

    [Serializable]
    public class HumanoidNeckDescriptor : IArtDescriptorGroup, IHumanoidDescriptor
    {
        public TransformArtReference head;
        public TransformArtReference upperNeck;
        public TransformArtReference lowerNeck;

        public void AutoFillBones(Animator animator)
        {
            head = animator.GetBoneTransform(HumanBodyBones.Head);
            lowerNeck = animator.GetBoneTransform(HumanBodyBones.Neck);
        }
    }

    [Serializable]
    public class HumanoidSpineDescriptor : IArtDescriptorGroup, IHumanoidDescriptor
    {
        public TransformArtReference upperChest;
        public TransformArtReference chest;
        public TransformArtReference spine;
        public TransformArtReference hips;

        public void AutoFillBones(Animator animator)
        {
            upperChest = animator.GetBoneTransform(HumanBodyBones.UpperChest);
            chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        }
    }

    [Serializable]
    public class HumanoidFingerDescriptor : IArtDescriptorGroup, IHumanoidDescriptor {
        public TransformArtReference metaCarpal;
        public TransformArtReference proximal;
        public TransformArtReference middle;
        public TransformArtReference distal;

        [HideInInspector]
        public HumanBodyBones? proximalBone, middleBone, distalBone;

        public void AutoFillBones(Animator animator) {
            metaCarpal = new TransformArtReference();

            if (proximalBone.HasValue)
                proximal = animator.GetBoneTransform(proximalBone.Value);

            if (middleBone.HasValue)
                middle = animator.GetBoneTransform(middleBone.Value);

            if (distalBone.HasValue)
                distal = animator.GetBoneTransform(distalBone.Value);
        }

        public HumanoidFingerDescriptor(HumanBodyBones? proximal, HumanBodyBones? middle, HumanBodyBones? distal) {
            proximalBone = proximal;
            middleBone = middle;
            distalBone = distal;
        }
    }

    [Serializable]
    public class HumanoidHandDescriptor : IArtDescriptorGroup, IHumanoidDescriptor
    {
        public TransformArtReference hand;

        public HumanoidFingerDescriptor thumb;
        public HumanoidFingerDescriptor index;
        public HumanoidFingerDescriptor middle;
        public HumanoidFingerDescriptor ring;
        public HumanoidFingerDescriptor pinky;

        [HideInInspector]
        public bool isLeft = false;

        public void AutoFillBones(Animator animator) {
            if (isLeft)
            {
                hand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                (thumb = new HumanoidFingerDescriptor(HumanBodyBones.LeftThumbProximal, HumanBodyBones.LeftThumbIntermediate, HumanBodyBones.LeftThumbDistal)).AutoFillBones(animator);
                (index = new HumanoidFingerDescriptor(HumanBodyBones.LeftIndexProximal, HumanBodyBones.LeftIndexIntermediate, HumanBodyBones.LeftIndexDistal)).AutoFillBones(animator);
                (middle = new HumanoidFingerDescriptor(HumanBodyBones.LeftMiddleProximal, HumanBodyBones.LeftMiddleIntermediate, HumanBodyBones.LeftMiddleDistal)).AutoFillBones(animator);
                (ring = new HumanoidFingerDescriptor(HumanBodyBones.LeftRingProximal, HumanBodyBones.LeftRingIntermediate, HumanBodyBones.LeftRingDistal)).AutoFillBones(animator);
                (pinky = new HumanoidFingerDescriptor(HumanBodyBones.LeftLittleProximal, HumanBodyBones.LeftLittleIntermediate, HumanBodyBones.LeftLittleDistal)).AutoFillBones(animator);
            }
            else
            {
                hand = animator.GetBoneTransform(HumanBodyBones.RightHand);
                (thumb = new HumanoidFingerDescriptor(HumanBodyBones.RightThumbProximal, HumanBodyBones.RightThumbIntermediate, HumanBodyBones.RightThumbDistal)).AutoFillBones(animator);
                (index = new HumanoidFingerDescriptor(HumanBodyBones.RightIndexProximal, HumanBodyBones.RightIndexIntermediate, HumanBodyBones.RightIndexDistal)).AutoFillBones(animator);
                (middle = new HumanoidFingerDescriptor(HumanBodyBones.RightMiddleProximal, HumanBodyBones.RightMiddleIntermediate, HumanBodyBones.RightMiddleDistal)).AutoFillBones(animator);
                (ring = new HumanoidFingerDescriptor(HumanBodyBones.RightRingProximal, HumanBodyBones.RightRingIntermediate, HumanBodyBones.RightRingDistal)).AutoFillBones(animator);
                (pinky = new HumanoidFingerDescriptor(HumanBodyBones.RightLittleProximal, HumanBodyBones.RightLittleIntermediate, HumanBodyBones.RightLittleDistal)).AutoFillBones(animator);
            }
        }
    }

    [Serializable]
    public class HumanoidArmDescriptor : IArtDescriptorGroup, IHumanoidDescriptor
    {
        public TransformArtReference collarBone;
        public TransformArtReference shoulderBlade;
        public TransformArtReference upperArm;
        public TransformArtReference lowerArm;
        public TransformArtReference wrist;
        public TransformArtReference carpal;
        public HumanoidHandDescriptor hand;

        [HideInInspector]
        public bool isLeft = false;

        public void AutoFillBones(Animator animator)
        {
            collarBone = null;
            wrist = null;
            carpal = null;

            if (isLeft)
            {
                shoulderBlade = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
                upperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                lowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            }
            else
            {
                shoulderBlade = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
                upperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                lowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            }

            hand.AutoFillBones(animator);
        }

        public HumanoidArmDescriptor(bool isLeft)
        {
            this.isLeft = isLeft;

            hand = new HumanoidHandDescriptor() {
                isLeft = isLeft,
            };
        }
    }

    [Serializable]
    public class HumanoidLegDescriptor : IArtDescriptorGroup, IHumanoidDescriptor
    {
        public TransformArtReference upperLeg;
        public TransformArtReference lowerLeg;
        public TransformArtReference foot;
        public TransformArtReference toe;

        [HideInInspector]
        public bool isLeft = false;

        public void AutoFillBones(Animator animator)
        {
            if (isLeft)
            {
                upperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
                lowerLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
                foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                toe = animator.GetBoneTransform(HumanBodyBones.LeftToes);
            }
            else
            {
                upperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                lowerLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
                foot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                toe = animator.GetBoneTransform(HumanBodyBones.RightToes);
            }
        }

        public HumanoidLegDescriptor(bool isLeft)
        {
            this.isLeft = isLeft;
        }
    }


    [Serializable]
    public class HumanoidArtDescriptor : IArtDescriptor, IHumanoidDescriptor {
        public HumanoidNeckDescriptor neckDescriptor = new();
        public HumanoidSpineDescriptor spineDescriptor = new();

        public HumanoidArmDescriptor leftArmDescriptor = new(true);
        public HumanoidArmDescriptor rightArmDescriptor = new(false);

        public HumanoidLegDescriptor leftLegDescriptor = new(true);
        public HumanoidLegDescriptor rightLegDescriptor = new(false);

        public void AutoFillBones(Animator animator) {
            (neckDescriptor = new()).AutoFillBones(animator);
            (spineDescriptor = new()).AutoFillBones(animator);

            (leftArmDescriptor = new(true)).AutoFillBones(animator);
            (rightArmDescriptor = new(false)).AutoFillBones(animator);

            (leftLegDescriptor = new(true)).AutoFillBones(animator);
            (rightLegDescriptor = new(false)).AutoFillBones(animator);
        }
    }
}
