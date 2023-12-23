using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VAT.Packaging
{
#if UNITY_EDITOR
    [StaticContentIdentifier("Level", typeof(SceneAsset))]
#endif
    public class StaticLevelContent : StaticContent, ILevelContent
    {
        [SerializeField]
        private StaticCrystScene _mainAsset;

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
                    _mainAsset = new StaticCrystScene(value.AssetGUID);
                }
                else
                {
                    _mainAsset = value as StaticCrystScene;
                }
            }
        }

        public StaticCrystScene MainScene { get { return _mainAsset; } set { _mainAsset = value; } }

#if UNITY_EDITOR
        public override string EditorAssetGroup => "Level";

        public override Type EditorAssetType => typeof(SceneAsset);
#endif
    }
}
