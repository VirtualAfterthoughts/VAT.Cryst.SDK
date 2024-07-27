using System;

namespace VAT.Logic
{
    [Serializable]
    public struct Signal
    {
        public static readonly Signal Identity = new()
        {
            value = 0f,
        };

        public float value;
    }
}
