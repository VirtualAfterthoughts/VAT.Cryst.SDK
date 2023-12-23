using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace VAT.Packaging
{
    public class StaticGameObjectContent : StaticContentT<GameObject>
    {
        [SerializeField]
        private StaticCrystGameObject _mainAsset;

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
                    _mainAsset = new StaticCrystGameObject(value.AssetGUID);
                }
                else
                {
                    _mainAsset = value as StaticCrystGameObject;
                }
            }
        }

        public StaticCrystGameObject MainGameObject { get { return _mainAsset; } set { _mainAsset = value; } }
    }
}
