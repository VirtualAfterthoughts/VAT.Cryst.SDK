using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Cryst.State
{
    public class GenericState<T>
    {
        private T _state = default;
        public T State
        {
            get
            {
                return _state;
            }
            set
            {
                if (!CheckEquality(_state, value))
                {
                    _state = value;
                    OnStateChanged?.Invoke(value);
                }
            }
        }

        public event Action<T> OnStateChanged;

        public virtual bool CheckEquality(T first, T second)
        {
            return first?.Equals(second) ?? false;
        }
    }
}
