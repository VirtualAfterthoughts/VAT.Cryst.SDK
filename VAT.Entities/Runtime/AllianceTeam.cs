using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Entities
{
    [CreateAssetMenu(fileName = "New Alliance Team", menuName = "Cryst/Entities/Alliance Team")]
    public class AllianceTeam : ScriptableObject
    {
        [SerializeField]
        private AllianceTeamContentReference[] _allies;

        [SerializeField]
        private AllianceTeamContentReference[] _enemies;

        public AllianceTeamContentReference[] Allies => _allies;

        public AllianceTeamContentReference[] Enemies => _enemies;
    }
}
