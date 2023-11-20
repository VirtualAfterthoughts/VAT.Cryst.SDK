using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    public static class DynamicContentFactory
    {
        public static T Create<T>(ContentInfo info, Object asset) where T : DynamicContent
        {
            var content = ScriptableObject.CreateInstance<T>();
            content.name = info.Title;
            content.ContentInfo = info;
            content.DynamicAsset = new DynamicCrystAsset(asset);

            content.BuildAddress();

            AssetPackager.HookOnReady(() =>
            {
                AssetPackager.Instance.LoadContent(content);
            });

            return content;
        }
    }

    public abstract class DynamicContent : Content
    {
        private DynamicCrystAsset _mainAsset;
        public virtual DynamicCrystAsset DynamicAsset
        {
            get
            {
                return _mainAsset;
            }
            set
            {
                _mainAsset = value;
            }
        }

        public override IWeakAsset MainAsset => DynamicAsset;

        public override IPackage MainPackage
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void BuildAddress()
        {
            Address = Address.BuildAddress("Runtime", "Generated", Info.Title);
        }
    }

    public abstract class DynamicContentT<T> : DynamicContent, IContentT<T> where T : Object
    {
        private DynamicCrystAssetT<T> _mainAsset;
        public DynamicCrystAssetT<T> DynamicAssetT
        {
            get
            {
                return _mainAsset;
            }
            set
            {
                _mainAsset = value;
            }
        }

        public override DynamicCrystAsset DynamicAsset 
        { 
            get => DynamicAssetT; 
            set => DynamicAssetT = new DynamicCrystAssetT<T>(value.Asset); 
        }

        public IWeakAssetT<T> MainAssetT => DynamicAssetT;
    }
}
