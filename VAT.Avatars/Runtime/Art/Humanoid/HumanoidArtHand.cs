using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Art
{
    public class HumanoidArtHand : HumanoidArtBoneGroupT<HumanoidHandDescriptor, HumanoidHand, HumanoidPhysArm>
    {
        public override int BoneCount => 1;

        public ArtBone Hand => Bones[0];

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
            Hand.Solve(PhysGroup.Hand.Transform);

            for (var i = 0; i < _fingers.Length; i++) {
                _fingers[i].Solve();
            }
        }

        public void SolveDataFingers() {
            for (var i = 0; i < _fingers.Length; i++)
            {
                _fingers[i].SolveData();
            }
        }

        public override void WriteData(HumanoidHand dataGroup, HumanoidPhysArm physGroup) {
            base.WriteData(dataGroup, physGroup);

            int fingerIndex = 0;
            for (var i = 0; i < _fingers.Length; i++) {
                if (fingerIndex >= dataGroup.Fingers.Length) {
                    fingerIndex = 0;
                }

                _fingers[i].WriteData(dataGroup.Fingers[fingerIndex], physGroup);
                
                fingerIndex++;
            }
        }

        public override void WriteOffsets(HumanoidHand dataGroup) {
            Hand.WriteOffset(dataGroup.Hand);

            int fingerIndex = 0;
            for (var i = 0; i < _fingers.Length; i++) {
                if (fingerIndex >= dataGroup.Fingers.Length) {
                    fingerIndex = 0;
                }

                _fingers[i].WriteOffsets(dataGroup.Fingers[fingerIndex]);

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
