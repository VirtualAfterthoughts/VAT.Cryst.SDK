using Newtonsoft.Json.Linq;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using VAT.Serialization.JSON;
using VAT.Shared.Extensions;

using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VAT.Packaging
{
    public static class ContentFactory
    {
        public static Content Create(Type type)
        {
            if (!type.IsSubclassOf(typeof(Content)))
                throw new Exception("Type does not inherit from Content.");

            var content = ScriptableObject.CreateInstance(type) as Content;
            return content;
        }
    }

    public abstract class Content : Shippable, IJSONPackable, IContent
    {
        public abstract IPackage MainPackage { get; set; }
        public abstract IWeakAsset MainAsset { get; }

        [SerializeField]
        private ContentInfo _contentInfo;

        public override IShippableInfo Info { get => _contentInfo; set => _contentInfo = (ContentInfo)value; }
        public ContentInfo ContentInfo { get => _contentInfo; set => _contentInfo = value; }

        public void Pack(JSONPacker packer, JObject json)
        {
            json.Add("address", Address.ID);
            json.Add("title", Info.Title);
            json.Add("description", Info.Description);

            OnPack(packer, json);
        }

        protected virtual void OnPack(JSONPacker packer, JObject json) { }

        public void Unpack(JSONUnpacker unpacker, JToken token)
        {
            ContentInfo info = new();

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

            ContentInfo = info;

            OnUnpack(unpacker, json);
        }

        protected virtual void OnUnpack(JSONUnpacker unpacker, JObject json) { }
    }
}
