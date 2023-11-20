using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    [Serializable]
    public struct ContentInfo : IShippableInfo
    {
        [SerializeField]
        [Tooltip("The title of the content.")]
        private string _title;
        public string Title { get { return _title; } set { _title = value; } }

        [SerializeField]
        [Tooltip("The description of the content.")]
        private string _description;
        public string Description { get { return _description; } set { _description = value; } }

        [SerializeField]
        [Tooltip("Is this content unlockable?")]
        private bool _unlockable;
        public bool Unlockable { get { return _unlockable; } set { _unlockable = value; } }

        [SerializeField]
        [Tooltip("Is this content hidden in menus?")]
        private bool _hidden;
        public bool Hidden { get { return _hidden; } set { _hidden = value; } }
    }

    public interface IContent : IShippable
    {
        ContentInfo ContentInfo { get; set; }

        IPackage MainPackage { get; set; }
        IWeakAsset MainAsset { get; }
    }

    public interface IContentT<T> : IContent where T : Object
    {
        IWeakAssetT<T> MainAssetT { get; }
    }
}
