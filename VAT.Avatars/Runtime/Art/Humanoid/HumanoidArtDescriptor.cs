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
            head = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.Head));
            lowerNeck = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.Neck));
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
            upperChest = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.UpperChest));
            chest = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.Chest));
            spine = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.Spine));
            hips = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.Hips));
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
                proximal = new TransformArtReference(animator.GetBoneTransform(proximalBone.Value));

            if (middleBone.HasValue)
                middle = new TransformArtReference(animator.GetBoneTransform(middleBone.Value));

            if (distalBone.HasValue)
                distal = new TransformArtReference(animator.GetBoneTransform(distalBone.Value));
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
                hand = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.LeftHand));
                (thumb = new HumanoidFingerDescriptor(HumanBodyBones.LeftThumbProximal, HumanBodyBones.LeftThumbIntermediate, HumanBodyBones.LeftThumbDistal)).AutoFillBones(animator);
                (index = new HumanoidFingerDescriptor(HumanBodyBones.LeftIndexProximal, HumanBodyBones.LeftIndexIntermediate, HumanBodyBones.LeftIndexDistal)).AutoFillBones(animator);
                (middle = new HumanoidFingerDescriptor(HumanBodyBones.LeftMiddleProximal, HumanBodyBones.LeftMiddleIntermediate, HumanBodyBones.LeftMiddleDistal)).AutoFillBones(animator);
                (ring = new HumanoidFingerDescriptor(HumanBodyBones.LeftRingProximal, HumanBodyBones.LeftRingIntermediate, HumanBodyBones.LeftRingDistal)).AutoFillBones(animator);
                (pinky = new HumanoidFingerDescriptor(HumanBodyBones.LeftLittleProximal, HumanBodyBones.LeftLittleIntermediate, HumanBodyBones.LeftLittleDistal)).AutoFillBones(animator);
            }
            else
            {
                hand = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.RightHand));
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
            collarBone = default;
            wrist = default;
            carpal = default;

            if (isLeft)
            {
                shoulderBlade = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.LeftShoulder));
                upperArm = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm));
                lowerArm = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
            }
            else
            {
                shoulderBlade = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.RightShoulder));
                upperArm = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.RightUpperArm));
                lowerArm = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
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
                upperLeg = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
                lowerLeg = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
                foot = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.LeftFoot));
                toe = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.LeftToes));
            }
            else
            {
                upperLeg = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.RightUpperLeg));
                lowerLeg = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
                foot = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.RightFoot));
                toe = new TransformArtReference(animator.GetBoneTransform(HumanBodyBones.RightToes));
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
