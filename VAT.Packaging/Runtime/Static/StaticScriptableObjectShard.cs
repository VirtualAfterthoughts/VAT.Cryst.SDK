using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [StaticShardIdentifier("Scriptable Object", typeof(ScriptableObject))]
    public class StaticScriptableObjectShard : StaticShardT<ScriptableObject>, IScriptableObjectShard
    {
        [SerializeField]
        private StaticCrystScriptableObject _mainAsset;

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
                    _mainAsset = new StaticCrystScriptableObject(value.AssetGUID);
                }
                else
                {
                    _mainAsset = value as StaticCrystScriptableObject;
                }
            }
        }

        public StaticCrystScriptableObject MainScriptableObject { get { return _mainAsset; } set { _mainAsset = value; } }
    }
}
