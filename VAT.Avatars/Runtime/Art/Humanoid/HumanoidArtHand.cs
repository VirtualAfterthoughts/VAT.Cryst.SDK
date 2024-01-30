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

        public ArtBone Hand => Bones[0] as ArtBone;

        private HumanoidArtFinger[] _fingers = null;
        public HumanoidArtFinger[] Fingers => _fingers;

        public override void Initiate() {
            base.Initiate();

            _fingers = new HumanoidArtFinger[5];
            for (var i = 0; i < _fingers.Length; i++) {
                _fingers[i] = new HumanoidArtFinger();
                _fingers[i].Initiate();
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
        }

        public override void WriteData(IHandGroup boneGroup) {
            base.WriteData(boneGroup);

            int fingerIndex = 0;
            for (var i = 0; i < _fingers.Length; i++) {
                if (fingerIndex >= boneGroup.Fingers.Length) {
                    fingerIndex = 0;
                }
            
                _fingers[i].WriteData(boneGroup.Fingers[fingerIndex]);
                
                fingerIndex++;
            }
        }

        public override void WriteOffsets(IHandGroup boneGroup) {
            Hand.WriteOffset(boneGroup.Hand);

            int fingerIndex = 0;
            for (var i = 0; i < _fingers.Length; i++) {
                if (fingerIndex >= boneGroup.Fingers.Length) {
                    fingerIndex = 0;
                }
            
                _fingers[i].WriteOffsets(boneGroup.Fingers[fingerIndex]);
            
                fingerIndex++;
            }
        }

        public override void WriteTransforms(HumanoidHandDescriptor artDescriptorGroup)
        {
            Hand.WriteReference(artDescriptorGroup.hand);

            _fingers[0].WriteTransforms(artDescriptorGroup.thumb);
            _fingers[1].WriteTransforms(artDescriptorGroup.index);
            _fingers[2].WriteTransforms(artDescriptorGroup.middle);
            _fingers[3].WriteTransforms(artDescriptorGroup.ring);
            _fingers[4].WriteTransforms(artDescriptorGroup.pinky);
        }
    }
}
