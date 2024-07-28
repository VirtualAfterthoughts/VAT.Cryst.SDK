namespace VAT.Props
{
    public enum BoltState
    {
        CLOSED = 0,
        OPENING = 1,
        OPEN = 2,
        CLOSING = 3,
    }

    public delegate void BoltCallback(BoltState previous, BoltState current);

    public interface IBolt
    {
        BoltState State { get; }

        event BoltCallback OnStateChanged;

        float OpenedPercent { get; }

        float TargetPercent { get; set; }

        bool Locked { get; set; }

        bool Overriden { get; set; }

        void ResetTarget();
    }
}
