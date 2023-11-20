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
    public struct PackageInfo : IShippableInfo
    {
        [SerializeField]
        [Tooltip("The title of the package.")]
        private string _title;
        public string Title { get { return _title; } set { _title = value; } }

        [SerializeField]
        [Tooltip("The description of the package.")]
        private string _description;
        public string Description { get { return _description; } set { _description = value; } }

        [SerializeField]
        [Tooltip("The author of the package.")]
        private string _author;
        public string Author { get { return _author; } set { _author = value; } }

    }

    public struct PackageLoadOptions
    {
        public bool isInternal;
    }

    public class Package : Shippable, IJSONPackable, IPackage
    {
        public const string BUILT_NAME = "package.json";

        private bool _isInternal;
        public bool IsInternal { get { return _isInternal; } }

        [SerializeField]
        [Tooltip("The contents packed within this package.")]
        private List<Content> _contents;
        public List<Content> Contents
        {
            get
            {
                _contents ??= new List<Content>();

                return _contents;
            }
            set { _contents = value; }
        }

        [SerializeField]
        private PackageInfo _packageInfo;

        public override IShippableInfo Info { get => _packageInfo; set => _packageInfo = (PackageInfo)value; }
        public PackageInfo PackageInfo { get => _packageInfo; set => _packageInfo = value; }

        public static Package Create(Type type)
        {
            if (!type.IsSubclassOf(typeof(Package)) && !(type == typeof(Package)))
                throw new Exception("Type does not inherit from Package.");

            var package = CreateInstance(type) as Package;
            return package;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            // Verify contents
            int count = _contents.RemoveAll((c) => c == null);

            if (count > 0)
                this.ForceSerialize();
        }
#endif

        public override void BuildAddress()
        {
            Address = Address.BuildAddress(PackageInfo.Author, "Package", PackageInfo.Title);
        }

        public void Load(PackageLoadOptions options)
        {
            _isInternal = options.isInternal;
        }

        public void Pack(JSONPacker packer, JObject json)
        {
            json.Add("address", Address.ID);
            json.Add("title", PackageInfo.Title);
            json.Add("description", PackageInfo.Description);
            json.Add("author", PackageInfo.Author);

            JArray contentArray = new();
            foreach (var content in _contents)
            {
                contentArray.Add(packer.PackReference(content));
            }
            json.Add("contents", contentArray);
        }

        public void Unpack(JSONUnpacker unpacker, JToken token)
        {
            PackageInfo info = new();

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

            if (json.TryGetValue("contents", out var contents))
            {
                _contents = new List<Content>();

                var contentArray = (JArray)contents;
                foreach (var reference in contentArray)
                {
                    if (unpacker.TryCreateFromReference(reference, out var content, ContentFactory.Create))
                    {
                        _contents.Add(content);
                    }
                }
            }

            PackageInfo = info;
        }
    }
}
