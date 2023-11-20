using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    public class StaticContentIdentifierAttribute : Attribute
    {
        public string displayName;
        public Type mainAssetType;

        public StaticContentIdentifierAttribute(string displayName, Type mainAssetType)
        {
            this.displayName = displayName;
            this.mainAssetType = mainAssetType;
        }
    }
}
