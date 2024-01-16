using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input
{
    public readonly struct CrystInputCallback
    {
        private readonly CrystInputPhase _phase;

        private readonly object _value;

        public CrystInputPhase Phase => _phase;

        public object Value => _value;

        public bool Started => Phase == CrystInputPhase.STARTED;
        public bool Performed => Phase == CrystInputPhase.PERFORMED;
        public bool Canceled => Phase == CrystInputPhase.CANCELED;

        public CrystInputCallback(CrystInputPhase phase, object value)
        {
            _phase = phase;
            _value = value;
        }

        public TValue GetValue<TValue>()
        {
            return (TValue)Value;
        }
    }
}
