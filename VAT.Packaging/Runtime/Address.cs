using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class Address : IEquatable<Address>, ISerializationCallbackReceiver
    {
        public const string Separator = ".";
        public const string EMPTY = "author.type.name";

        [SerializeField]
        private string _id = EMPTY;

        [JsonProperty("id")]
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public Address(string id)
        {
            _id = id;
        }

        public Address(Address other)
        {
            _id = other._id;
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            if (!IsValid(ID))
                _id = EMPTY;
        }

        public bool Equals(Address other) => _id == other._id;

        public override bool Equals(object obj)
        {
            if (obj is not Address other) 
            { 
                return false; 
            }

            return Equals(other);
        }

        public override int GetHashCode()
        {
            if (_id == null)
                return 0;

            return _id.GetHashCode();
        }

        public static Address CreateEmptyAddress() => new(EMPTY);

        public static bool IsValid(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return false;
            }
            else if (address == EMPTY)
            {
                return false;
            }

            return true;
        }

        public static string BuildAddress(params string[] parts)
        {
            StringBuilder builder = new();
            bool includePrefix = false;

            foreach (var part in parts)
            {
                if (includePrefix)
                    builder.Append(Separator);
                includePrefix = true;

                builder.Append(part);
            }

            return CleanAddress(builder.ToString());
        }

        public static string CleanAddress(string address)
        {
            StringBuilder sb = new();
            foreach (char c in address)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
                else
                    sb.Append("_");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return ID;
        }

        public static bool operator ==(Address lhs, Address rhs)
        {
            bool bothNull = lhs is null && rhs is null;
            bool oneNull = lhs is null || rhs is null;

            if (bothNull)
            {
                return true;
            }

            if (oneNull && !bothNull)
            {
                return false;
            }

            return lhs.ID == rhs.ID;
        }

        public static bool operator !=(Address lhs, Address rhs)
        {
            bool bothNull = lhs is null && rhs is null;
            bool oneNull = lhs is null || rhs is null;

            if (bothNull)
            {
                return true;
            }

            if (oneNull && !bothNull)
            {
                return false;
            }

            return lhs.ID != rhs.ID;
        }

    }
}
