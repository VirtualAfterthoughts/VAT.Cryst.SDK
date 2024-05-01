using VAT.Avatars.REWORK;

using VAT.Input;

namespace VAT.Avatars.Integumentary {
    public struct AvatarLeg {
        public Handedness Handedness { get; }
        public IBone DataRig { get; }
        public IBone PhysRig { get; }
        public ILegGroup DataLeg { get; }
        public ILegGroup PhysLeg { get; }

        public AvatarLeg(Handedness handedness, IBone dataRig, IBone physRig, ILegGroup dataLeg, ILegGroup physLeg)
        {
            Handedness = handedness;
            DataRig = dataRig;
            PhysRig = physRig;
            DataLeg = dataLeg;
            PhysLeg = physLeg;
        }
    }
}
