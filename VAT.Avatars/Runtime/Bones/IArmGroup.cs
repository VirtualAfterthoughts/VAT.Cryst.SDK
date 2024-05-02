namespace VAT.Avatars.Bones
{
    public interface IArmGroup : ILimbGroup
    {
        IBone UpperArm { get; }
        IBone Elbow { get; }
        IHandGroup Hand { get; }
    }
}
