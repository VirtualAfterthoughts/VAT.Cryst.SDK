using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    public interface IContentReference
    {
        Address Address { get; }
        bool TryGetContent(out IContent content);
    }

    [Serializable]
    public class ContentReference : IContentReference
    {
        [SerializeField]
        protected Address _address = Address.EMPTY;

        public Address Address { get { return _address; } set { _address = value; } }

#if UNITY_EDITOR
        public virtual Type EditorContentType => typeof(Content);
#endif

        public bool TryGetContent(out IContent content)
        {
            content = null;

            if (!AssetPackager.IsReady)
                return false;

            return AssetPackager.Instance.TryGetContent(Address, out content);
        }
    }

    [Serializable]
    public class ContentReferenceT<T> : ContentReference where T : IContent
    {
        public bool TryGetContent(out T content)
        {
            content = default;
            base.TryGetContent(out var otherContent);

            if (otherContent is T cast)
            {
                content = cast;
                return true;
            }

            return false;
        }
    }
}
