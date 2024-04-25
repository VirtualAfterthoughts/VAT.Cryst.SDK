using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Entities
{
    [CreateAssetMenu(fileName = "New Alliance Team", menuName = "Cryst/Entities/Alliance Team")]
    public class AllianceTeam : ScriptableObject
    {
        [SerializeField]
        private AllianceTeamShardReference[] _allies;

        [SerializeField]
        private AllianceTeamShardReference[] _enemies;

        public AllianceTeamShardReference[] Allies => _allies;

        public AllianceTeamShardReference[] Enemies => _enemies;
    }
}
