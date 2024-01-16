using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input
{
    public class CrystInputAction
    {
        public event Action<CrystInputCallback> Started, Performed, Canceled;

        protected CrystInputPhase _phase;
        public CrystInputPhase Phase
        {
            get
            {
                return _phase;
            }
            set
            {
                SetPhase(value);
            }
        }

        protected object _value;
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                SetValue(value);
            }
        }

        public TValue GetValue<TValue>()
        {
            if (_value == null)
                return default;

            return (TValue)_value;
        }

        public void SetPhase(CrystInputPhase phase)
        {
            _phase = phase;

            var callback = CreateCallback();

            switch (phase)
            {
                case CrystInputPhase.STARTED:
                    Started?.Invoke(callback);
                    break;
                case CrystInputPhase.PERFORMED:
                    Performed?.Invoke(callback);
                    break;
                case CrystInputPhase.CANCELED:
                    Canceled?.Invoke(callback);
                    break;
            }
        }

        public void SetValue(object value)
        {
            _value = value;
        }

        public CrystInputCallback CreateCallback()
        {
            return new CrystInputCallback(Phase, Value);
        }
    }

    public class CrystInputActionT<TValue> : CrystInputAction
    {
        public new TValue Value
        { 
            get
            {
                return GetValue<TValue>();
            }
            set
            {
                SetValue(value);
            }
        }

        public void SetValueGeneric(TValue value)
        {
            base.SetValue(value);
        }
    }
}
