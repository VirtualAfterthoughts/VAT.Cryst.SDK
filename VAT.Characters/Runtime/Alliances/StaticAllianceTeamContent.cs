using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Packaging;

namespace VAT.Entities
{
    [StaticContentIdentifier("Alliance Team", typeof(AllianceTeam))]
    public class StaticAllianceTeamContent : StaticContentT<AllianceTeam>
    {
        [SerializeField]
        private StaticCrystAllianceTeam _mainAsset;

        public override StaticCrystAsset StaticAsset
        {
            get
            {
                return _mainAsset;
            }
            set
            {
                if (value != null && value.GetType() == typeof(StaticCrystAsset))
                {
                    _mainAsset = new StaticCrystAllianceTeam(value.AssetGUID);
                }
                else
                {
                    _mainAsset = value as StaticCrystAllianceTeam;
                }
            }
        }

        public StaticCrystAllianceTeam MainAllianceTeam { get { return _mainAsset; } set { _mainAsset = value; } }
    }
}
