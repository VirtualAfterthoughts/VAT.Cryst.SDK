using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VAT.Packaging;

namespace VAT.Entities
{
    [Serializable]
    public class AllianceTeamShardReference : ShardReferenceT<StaticAllianceTeamContent>
    {
#if UNITY_EDITOR
        public override Type EditorShardType => typeof(StaticAllianceTeamContent);
#endif
    }
}
