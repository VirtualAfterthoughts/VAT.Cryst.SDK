using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Shared.Utilities {
    public abstract class DerivativeTracker : IResetable {
        public DerivativeTracker(int count) {
            SetMaxDerivative(count);
        }

        public abstract void Reset();

        public abstract void SetMaxDerivative(int count);
    }

    public abstract class DerivativeTrackerT<TValue> : DerivativeTracker {
        public virtual TValue Default => default;

        protected TValue[] _buffer;
        protected TValue[] _array;
        protected int _length;

        public DerivativeTrackerT(int count) : base(count) { }

        public override void Reset() {
            for (var i = 0; i < _length; i++) {
                _array[i] = Default;
            }
        }

        public override void SetMaxDerivative(int count) {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count");

            _length = count + 1;
            _array = new TValue[_length];
            _buffer = new TValue[_length];
        }

        public void Update(TValue current, float deltaTime) {
            _buffer[0] = current;
            for (var i = 1; i < _length; i++) {
                _buffer[i] = CalculateDerivative(_array[i-1], _buffer[i-1], deltaTime);
            }

            for (var i = 0; i < _length; i++) {
                _array[i] = _buffer[i];
            }
        }

        protected abstract TValue CalculateDerivative(TValue from, TValue to, float deltaTime);

        public TValue GetDerivative(int position) {
            if (position >= _length)
                throw new IndexOutOfRangeException();

            return _array[position];
        }

        public void SetDerivative(int position, TValue value) {
            if (position >= _length)
                throw new IndexOutOfRangeException();

            _array[position] = value;
        }
    }
}
