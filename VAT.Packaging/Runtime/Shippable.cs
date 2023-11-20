using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace VAT.Packaging
{
    public interface IShippableInfo
    {
        string Title { get; }
        string Description { get; }
    }

    public interface IReadOnlyShippable
    {
        Address Address { get; }
    }

    public interface IShippable : IReadOnlyShippable
    {
        IShippableInfo Info { get; set; }
    }

    public abstract class Shippable : ScriptableObject, IShippable
    {
        [SerializeField]
        protected Address _address;
        public virtual Address Address { get { return _address; } set { _address = value; } }

        public abstract IShippableInfo Info { get; set; }

        public abstract void BuildAddress();

#if UNITY_EDITOR
        [ContextMenu("Build Address")]
        private void ContextBuildAddress()
        {
            if (EditorUtility.DisplayDialog(
                "Build Address", 
                "Are you sure you want to rebuild the address? This will break all existing references!",
                "Build"))
            {
                BuildAddress();
            }
        }
#endif
    }
}
