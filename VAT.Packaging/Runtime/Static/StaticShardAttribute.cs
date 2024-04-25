using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    public class StaticShardIdentifierAttribute : Attribute
    {
        public string displayName;
        public Type mainAssetType;

        public StaticShardIdentifierAttribute(string displayName, Type mainAssetType)
        {
            this.displayName = displayName;
            this.mainAssetType = mainAssetType;
        }
    }
}
