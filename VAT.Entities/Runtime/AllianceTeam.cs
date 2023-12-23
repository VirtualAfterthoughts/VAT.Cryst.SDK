using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Entities
{
    public enum DefaultAlliance
    {
        NEUTRAL = 1 << 0,
        FRIEND = 1 << 1,
        ENEMY = 1 << 2,
    }

    [CreateAssetMenu(fileName = "New Alliance Team", menuName = "Cryst/Entities/Alliance Team")]
    public class AllianceTeam : ScriptableObject
    {
        [SerializeField]
        private AllianceTeamContentReference[] _allies;

        [SerializeField]
        private AllianceTeamContentReference[] _enemies;

        [SerializeField]
        private DefaultAlliance _defaultAlliance = DefaultAlliance.NEUTRAL;

        public AllianceTeamContentReference[] Allies => _allies;

        public AllianceTeamContentReference[] Enemies => _enemies;

        public DefaultAlliance DefaultAlliance => _defaultAlliance;
    }
}
