namespace VAT.Cryst.Interfaces
{
    public interface IActuatable
    {
        bool IsActuated { get; }

        void Actuate(bool actuated = true);
    }
}
