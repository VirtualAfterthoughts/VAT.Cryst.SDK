using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

namespace VAT.Entities
{
    [Serializable]
    public class StaticCrystAllianceTeam : StaticCrystAssetT<AllianceTeam>
    {
        public StaticCrystAllianceTeam(string guid) : base(guid) { }
    }
}
