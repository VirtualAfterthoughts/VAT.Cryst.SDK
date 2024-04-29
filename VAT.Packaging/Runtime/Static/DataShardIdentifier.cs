using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    public class DataShardIdentifier : Attribute
    {
        public string displayName;

        public DataShardIdentifier(string displayName)
        {
            this.displayName = displayName;
        }
    }
}
