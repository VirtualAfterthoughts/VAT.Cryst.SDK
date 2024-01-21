using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Skeletal;
using VAT.Avatars.Nervous;

using VAT.Shared.Data;

using Avatar = VAT.Avatars.Integumentary.Avatar;

using System.Linq;
using VAT.Input;

namespace VAT.Avatars.Example
{
    public sealed class BasicAvatarRig : MonoBehaviour {
        public struct BasicHand : IHand
        {
            public SimpleTransform Transform { get { return _transform; } set { _transform = value; } }

            private SimpleTransform _transform;

            public BasicHand(SimpleTransform transform)
            {
                _transform = transform;
            }

            public bool TryGetInputController(out IInputController input)
            {
                input = default;
                return false;
            }
        }

        public struct BasicArm : IArm
        {
            private IJoint[] _bones;

            public BasicArm(params SimpleTransform[] transforms)
            {
                var bones = new IJoint[transforms.Length];

                for (var i = 0; i < bones.Length; i++)
                {
                    if (i <= 0)
                    {
                        bones[i] = new BasicHand(transforms[i]);
                        continue;
                    }

                    bones[i] = new BasicJoint(transforms[i]);
                }

                _bones = bones;
            }

            public readonly int JointCount => 1;

            public readonly IJoint GetJoint(int index)
            {
                return _bones.ElementAt(index);
            }

            public void SetJoint(int index, IJoint joint)
            {
                _bones[index] = joint;
            }

            public readonly bool TryGetElbow(out IJoint elbow)
            {
                if (_bones.Length > 1)
                {
                    elbow = _bones.ElementAt(1);
                    return true;
                }

                elbow = default;
                return false;
            }

            public readonly bool TryGetHand(out IHand hand)
            {
                hand = _bones.ElementAt(0) as IHand;
                return true;
            }

            public readonly bool TryGetUpperArm(out IJoint upperArm)
            {
                if (_bones.Length > 2)
                {
                    upperArm = _bones.ElementAt(2);
                    return true;
                }

                upperArm = default;
                return false;
            }
        }

        public class AvatarInput : IBasicInput
        {
            public AvatarInput()
            {
            }

            bool IBasicInput.GetJump()
            {
                return false;
            }

            Vector3 IBasicInput.GetMovement()
            {
                return Vector3.zero;
            }
        }

        public Avatar avatar;

        public Transform head;

        public Transform leftHand;

        public Transform leftElbow;

        public Transform rightHand;

        public Transform rightElbow;

        private AvatarInput _input;

        public void Awake() {
            _input = new AvatarInput();

            avatar.Initiate();
        }

        public void FixedUpdate() {
            var leftArm = leftElbow ? new BasicArm(leftHand, leftElbow) : new BasicArm(leftHand);
            var rightArm = rightElbow ? new BasicArm(rightHand, rightElbow) : new BasicArm(rightHand);

            var payload = new BasicAvatarPayload() {
                Root = SimpleTransform.Create(transform),
                Head = SimpleTransform.Create(head),
                LeftArm = leftArm,
                RightArm = rightArm,
                Input = _input,
            };

            avatar.Write(payload);
            avatar.Anatomy.Skeleton.DataBoneSkeleton.Solve();
            avatar.Anatomy.Skeleton.PhysBoneSkeleton.Solve();
        }

        public void LateUpdate() {
            avatar.SolveArt();
        }
    }
}
