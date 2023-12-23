using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Entities
{
    [CreateAssetMenu]
    public class AllianceData : ScriptableObject
    {
        [SerializeField]
        private AllianceTeamContentReference _team;
    }
}
