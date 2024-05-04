using Newtonsoft.Json.Linq;

using System;

using UnityEngine;

using VAT.Serialization.JSON;

namespace VAT.Packaging
{
    public static class ShardFactory
    {
        public static Shard Create(Type type)
        {
            if (!type.IsSubclassOf(typeof(Shard)))
                throw new Exception("Type does not inherit from Shard.");

            var content = ScriptableObject.CreateInstance(type) as Shard;
            return content;
        }
    }

    public abstract class Shard : Shippable, IJSONPackable, IShard
    {
        public abstract ICrystal MainCrystal { get; set; }

        [SerializeField]
        private ShardInfo _shardInfo;

        [SerializeField]
        private string _addressType;
        public virtual string AddressType
        {
            get
            {
                return _addressType;
            }
            set
            {
                _addressType = value;
            }
        }

        public override IShippableInfo Info { get => _shardInfo; set => _shardInfo = (ShardInfo)value; }
        public ShardInfo ShardInfo { get => _shardInfo; set => _shardInfo = value; }

        public void Pack(JSONPacker packer, JObject json)
        {
            json.Add("address", Address.ID);
            json.Add("title", Info.Title);
            json.Add("description", Info.Description);
            json.Add("unlockable", ShardInfo.Unlockable);
            json.Add("hidden", ShardInfo.Hidden);

            OnPack(packer, json);
        }

        protected abstract void OnPack(JSONPacker packer, JObject json);

        public void Unpack(JSONUnpacker unpacker, JToken token)
        {
            ShardInfo info = new();

            var json = (JObject)token;

            if (json.TryGetValue("address", out var address))
            {
                _address = new Address(address.ToString());
            }

            if (json.TryGetValue("title", out var title))
            {
                info.Title = title.ToString();
                name = $"_{info.Title}";
            }

            if (json.TryGetValue("description", out var description))
            {
                info.Description = description.ToString();
            }

            if (json.TryGetValue("unlockable", out var unlockable))
            {
                info.Unlockable = unlockable.ToObject<bool>();
            }

            if (json.TryGetValue("hidden", out var hidden))
            {
                info.Hidden = hidden.ToObject<bool>();
            }

            ShardInfo = info;

            OnUnpack(unpacker, json);
        }

        protected abstract void OnUnpack(JSONUnpacker unpacker, JObject json);

        public override void BuildAddress()
        {
            var crystalInfo = MainCrystal.CrystalInfo;

            if (!string.IsNullOrWhiteSpace(AddressType))
            {
                Address = Address.BuildAddress(crystalInfo.Author, crystalInfo.Title, AddressType, ShardInfo.Title);
            }
            else
            {
                Address = Address.BuildAddress(crystalInfo.Author, crystalInfo.Title, ShardInfo.Title);
            }
        }
    }
}
