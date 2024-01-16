using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Skeletal;
using VAT.Avatars.Nervous;

using VAT.Shared.Data;

using Avatar = VAT.Avatars.Integumentary.Avatar;
using VAT.Input.Desktop;

namespace VAT.Avatars.Example
{
    public sealed class BasicAvatarRig : MonoBehaviour {
        public Avatar avatar;

        public Transform head;

        public Transform leftHand;

        public Transform rightHand;

        private DesktopInput _input;

        public void Awake() {
            _input = new DesktopInput();

            avatar.Initiate();
        }

        public void FixedUpdate() {
            var payload = new BasicAvatarPayload() {
                Root = SimpleTransform.Create(transform),
                Head = SimpleTransform.Create(head),
                LeftHand = SimpleTransform.Create(leftHand),
                RightHand = SimpleTransform.Create(rightHand),
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
