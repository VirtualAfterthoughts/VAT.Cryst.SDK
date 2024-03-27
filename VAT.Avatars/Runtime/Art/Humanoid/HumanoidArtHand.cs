using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.REWORK;
using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Art
{
    public class HumanoidArtHand : HumanoidArtBoneGroupT<HumanoidHandDescriptor, IHandGroup>
    {
        public override int BoneCount => 1;

        public ArtBone Hand => Bones[0];

        private HumanoidArtThumb[] _thumbs = null;
        public HumanoidArtThumb[] Thumbs => _thumbs;

        private HumanoidArtFinger[] _fingers = null;
        public HumanoidArtFinger[] Fingers => _fingers;

        public override void Initiate() {
            base.Initiate();

            _thumbs = new HumanoidArtThumb[1];
            for (var i = 0; i < _thumbs.Length; i++)
            {
                _thumbs[i] = new HumanoidArtThumb();
                _thumbs[i].Initiate();
            }

            _fingers = new HumanoidArtFinger[4];
            for (var i = 0; i < _fingers.Length; i++) {
                _fingers[i] = new HumanoidArtFinger();
                _fingers[i].Initiate();
            }
        }

        public override void Deinitiate()
        {
            base.Deinitiate();

            foreach (var finger in Fingers)
            {
                finger.Deinitiate();
            }

            foreach (var thumb in Thumbs)
            {
                thumb.Deinitiate();
            }
        }

        public override void Solve()
        {
            Hand.Solve(BoneGroup.Hand.Transform);

            SolveFingers();
        }

        public void SolveFingers() {
            for (var i = 0; i < _fingers.Length; i++)
            {
                _fingers[i].Solve();
            }

            for (var i = 0; i < _thumbs.Length; i++)
            {
                _thumbs[i].Solve();
            }
        }

        public override void WriteData(IHandGroup boneGroup) {
            base.WriteData(boneGroup);

            for (var i = 0; i < _fingers.Length; i++)
            {
                _fingers[i].WriteData(boneGroup.Fingers[i]);
            }

            for (var i = 0; i < _thumbs.Length; i++)
            {
                _thumbs[i].WriteData(boneGroup.Thumbs[i]);
            }
        }

        public override void WriteOffsets(IHandGroup boneGroup) {
            Hand.WriteOffset(boneGroup.Hand);

            for (var i = 0; i < _fingers.Length; i++) {
                _fingers[i].WriteOffsets(boneGroup.Fingers[i]);
            }

            for (var i = 0; i < _thumbs.Length; i++)
            {
                _thumbs[i].WriteOffsets(boneGroup.Thumbs[i]);
            }
        }

        public override void WriteTransforms(HumanoidHandDescriptor artDescriptorGroup)
        {
            Hand.WriteReference(artDescriptorGroup.hand);

            _thumbs[0].WriteTransforms(artDescriptorGroup.thumb);

            _fingers[0].WriteTransforms(artDescriptorGroup.index);
            _fingers[1].WriteTransforms(artDescriptorGroup.middle);
            _fingers[2].WriteTransforms(artDescriptorGroup.ring);
            _fingers[3].WriteTransforms(artDescriptorGroup.pinky);
        }
    }
}
