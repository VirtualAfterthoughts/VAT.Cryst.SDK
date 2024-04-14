namespace VAT.Cryst.State
{
    public sealed class BoolState : GenericState<bool>
    {
        public override bool CheckEquality(bool first, bool second)
        {
            return first == second;
        }
    }
}
