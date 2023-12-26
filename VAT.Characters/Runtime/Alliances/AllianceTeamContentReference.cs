using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VAT.Packaging;

namespace VAT.Entities
{
    [Serializable]
    public class AllianceTeamContentReference : ContentReferenceT<StaticAllianceTeamContent>
    {
#if UNITY_EDITOR
        public override Type EditorContentType => typeof(StaticAllianceTeamContent);
#endif
    }
}
