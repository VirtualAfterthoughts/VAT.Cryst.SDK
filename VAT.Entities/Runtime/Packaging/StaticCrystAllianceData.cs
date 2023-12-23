using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

namespace VAT.Entities
{
    [Serializable]
    public class StaticCrystAllianceData : StaticCrystAssetT<AllianceData>
    {
        public StaticCrystAllianceData(string guid) : base(guid) { }
    }
}
