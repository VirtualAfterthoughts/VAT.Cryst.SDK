namespace VAT.Cryst
{
    public interface IActuatable
    {
        bool IsActuated { get; }

        void Actuate(bool actuated = true);
    }
}
