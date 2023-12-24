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

    [CreateAssetMenu(fileName = "New Alliance Data", menuName = "Cryst/Entities/Alliance Data")]
    public class AllianceData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The team assigned to this entity alliance.")]
        private AllianceTeamContentReference _team;

        [SerializeField]
        [Tooltip("Is this entity an ally of its own team?")]
        private bool _allyWithTeam = true;

        [SerializeField]
        private DefaultAlliance _defaultAlliance = DefaultAlliance.NEUTRAL;

        public AllianceTeamContentReference Team { get { return _team; } set { _team = value; } }

        public bool AllyWithTeam { get { return _allyWithTeam; } set { _allyWithTeam = value; } }

        public DefaultAlliance DefaultAlliance => _defaultAlliance;
    }
}
