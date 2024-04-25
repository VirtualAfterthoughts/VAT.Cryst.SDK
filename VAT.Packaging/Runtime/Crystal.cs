using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using VAT.Serialization.JSON;
using VAT.Shared.Extensions;

namespace VAT.Packaging
{
    [Serializable]
    public struct CrystalInfo : IShippableInfo
    {
        [SerializeField]
        [Tooltip("The title of the crystal.")]
        private string _title;
        public string Title { get { return _title; } set { _title = value; } }

        [SerializeField]
        [Tooltip("The description of the crystal.")]
        private string _description;
        public string Description { get { return _description; } set { _description = value; } }

        [SerializeField]
        [Tooltip("The author of the crystal.")]
        private string _author;
        public string Author { get { return _author; } set { _author = value; } }

    }

    public struct CrystalLoadOptions
    {
        public bool isInternal;
    }

    public class Crystal : Shippable, IJSONPackable, ICrystal
    {
        public const string BUILT_NAME = "crystal.json";

        private bool _isInternal;
        public bool IsInternal { get { return _isInternal; } }

        [SerializeField]
        [Tooltip("The shards making up this crystal.")]
        private List<Shard> _shards;
        public List<Shard> Shards
        {
            get
            {
                _shards ??= new List<Shard>();

                return _shards;
            }
            set { _shards = value; }
        }

        [SerializeField]
        private CrystalInfo _crystalInfo;

        public override IShippableInfo Info { get => _crystalInfo; set => _crystalInfo = (CrystalInfo)value; }
        public CrystalInfo CrystalInfo { get => _crystalInfo; set => _crystalInfo = value; }

        public static Crystal Create(Type type)
        {
            if (!type.IsSubclassOf(typeof(Crystal)) && !(type == typeof(Crystal)))
                throw new Exception("Type does not inherit from Crystal.");

            var package = CreateInstance(type) as Crystal;
            return package;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            // Verify shards
            int count = _shards.RemoveAll((c) => c == null);

            if (count > 0)
                this.ForceSerialize();
        }
#endif

        public override void BuildAddress()
        {
            Address = Address.BuildAddress(CrystalInfo.Author, "Crystal", CrystalInfo.Title);
        }

        public void Load(CrystalLoadOptions options)
        {
            _isInternal = options.isInternal;
        }

        public void Pack(JSONPacker packer, JObject json)
        {
            json.Add("address", Address.ID);
            json.Add("title", CrystalInfo.Title);
            json.Add("description", CrystalInfo.Description);
            json.Add("author", CrystalInfo.Author);

            JArray shardArray = new();
            foreach (var shard in _shards)
            {
                shardArray.Add(packer.PackReference(shard));
            }
            json.Add("shards", shardArray);
        }

        public void Unpack(JSONUnpacker unpacker, JToken token)
        {
            CrystalInfo info = new();

            var json = (JObject)token;

            if (json.TryGetValue("address", out var address))
            {
                _address = new Address(address.ToString());
            }

            if (json.TryGetValue("title", out var title))
            {
                info.Title = title.ToString();
                name = info.Title;
            }

            if (json.TryGetValue("description", out var description))
            {
                info.Description = description.ToString();
            }

            if (json.TryGetValue("author", out var author))
            {
                info.Author = author.ToString();
            }

            if (json.TryGetValue("shards", out var shards))
            {
                _shards = new List<Shard>();

                var shardArray = (JArray)shards;
                foreach (var reference in shardArray)
                {
                    if (unpacker.TryCreateFromReference(reference, out var shard, ShardFactory.Create))
                    {
                        _shards.Add(shard);
                    }
                }
            }

            CrystalInfo = info;
        }
    }
}
