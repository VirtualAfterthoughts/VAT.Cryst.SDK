using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Shared
{
    public static class SerializedNullableTExtensions {
        public static T GetValueOrDefault<T>(this SerializedNullableT<T> nullable) where T : struct {
            if (HasValue(nullable))
                return nullable.Value;
            else
                return default;
        }

        public static bool HasValue<T>(this SerializedNullableT<T> nullable) where T : struct {
            return nullable != null && nullable.Internal_GetHasValue();
        }
    }

    public abstract class SerializedNullableT<T> where T : struct {
        [HideInInspector]
        [SerializeField]
        protected T _value;

        [HideInInspector]
        [SerializeField]
        protected bool _hasValue;

        public T Value {
            get {
                if (!this.HasValue())
                    throw new NullReferenceException();

                return _value;
            }
        }

        protected SerializedNullableT(T value) {
            _value = value;
            _hasValue = true;
        }

        internal bool Internal_GetHasValue() => _hasValue;

        public override int GetHashCode() {
            return _value.GetHashCode();
        }

        public override bool Equals(object other)
        {
            SerializedNullableT<T> nullable = other as SerializedNullableT<T>;
            if (nullable == null && other != null && other is not SerializedNullableT<T>) {
                return false;
            }

            bool hasValue = this.HasValue();

            if (nullable == null && !hasValue)
                return true;
            else if (nullable == null && hasValue)
                return false;

            return _value.Equals(nullable._value);
        }

        public static implicit operator bool(SerializedNullableT<T> nullable) {
            return nullable.HasValue();
        }
    }

    [Serializable]
    public class SerializedNullableVector3 : SerializedNullableT<Vector3>
    {
        public SerializedNullableVector3(Vector3 value) : base(value) { }

        public static implicit operator SerializedNullableVector3(Vector3 value) => new(value);

        public static implicit operator Vector3?(SerializedNullableVector3 value) => value.HasValue() ? value.Value : null;
    }

    [Serializable]
    public class SerializedNullableQuaternion : SerializedNullableT<Quaternion>
    {
        public SerializedNullableQuaternion(Quaternion value) : base(value) { }

        public static implicit operator SerializedNullableQuaternion(Quaternion value) => new(value);

        public static implicit operator Quaternion?(SerializedNullableQuaternion value) => value.HasValue() ? value.Value : null;
    }

    [Serializable]
    public class SerializedNullableFloat : SerializedNullableT<float>
    {
        public SerializedNullableFloat(float value) : base(value) { }

        public static implicit operator SerializedNullableFloat(float value) => new(value);

        public static implicit operator float?(SerializedNullableFloat value) => value.HasValue() ? value.Value : null;
    }
}
