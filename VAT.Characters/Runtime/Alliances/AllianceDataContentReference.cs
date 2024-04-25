using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VAT.Packaging;

namespace VAT.Entities
{
    [Serializable]
    public class AllianceDataShardReference : ShardReferenceT<StaticAllianceDataContent>
    {
#if UNITY_EDITOR
        public override Type EditorShardType => typeof(StaticAllianceDataContent);
#endif
    }
}
