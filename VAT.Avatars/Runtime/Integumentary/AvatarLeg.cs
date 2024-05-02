using VAT.Avatars.Bones;

using VAT.Input;

namespace VAT.Avatars.Integumentary {
    public readonly struct AvatarLeg
    {
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

        public Handedness GetHandedness()
        {
            return Handedness;
        }

        public int GetIndex()
        {
            throw new System.NotImplementedException();
        }
    }
}
