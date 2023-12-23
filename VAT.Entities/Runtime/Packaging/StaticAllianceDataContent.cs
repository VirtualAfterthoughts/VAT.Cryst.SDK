using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Packaging;

namespace VAT.Entities
{
    [StaticContentIdentifier("Alliance Data", typeof(AllianceData))]
    public class StaticAllianceDataContent : StaticContentT<AllianceData>
    {
        [SerializeField]
        private StaticCrystAllianceData _mainAsset;

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
                    _mainAsset = new StaticCrystAllianceData(value.AssetGUID);
                }
                else
                {
                    _mainAsset = value as StaticCrystAllianceData;
                }
            }
        }

        public StaticCrystAllianceData MainAllianceTeam { get { return _mainAsset; } set { _mainAsset = value; } }
    }
}
