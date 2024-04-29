using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    public class StaticShardIdentifier : Attribute
    {
        public string displayName;
        public Type mainAssetType;

        public StaticShardIdentifier(string displayName, Type mainAssetType)
        {
            this.displayName = displayName;
            this.mainAssetType = mainAssetType;
        }
    }
}
