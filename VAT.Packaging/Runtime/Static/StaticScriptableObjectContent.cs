using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [StaticContentIdentifier("Scriptable Object", typeof(ScriptableObject))]
    public class StaticScriptableObjectContent : StaticContentT<ScriptableObject>, IScriptableObjectContent
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
