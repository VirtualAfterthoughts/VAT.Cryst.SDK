using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

namespace VAT.Props
{
    [StaticShardIdentifier("Surface Material", typeof(SurfaceMaterial))]
    public class SurfaceMaterialShard : StaticShardT<SurfaceMaterial>
    {
        [SerializeField]
        private StaticCrystAssetT<SurfaceMaterial> _mainAsset;

        public override StaticCrystAsset StaticAsset 
        { 
            get => _mainAsset; 
            set 
            {
                if (value != null && value.GetType() == typeof(StaticCrystAsset))
                {
                    _mainAsset = new StaticCrystAssetT<SurfaceMaterial>(value.AssetGUID);
                }
                else
                {
                    _mainAsset = value as StaticCrystAssetT<SurfaceMaterial>;
                }
            }
        }
    }
}
