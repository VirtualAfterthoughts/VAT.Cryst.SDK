using Newtonsoft.Json.Linq;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Serialization.JSON;

namespace VAT.Packaging
{
    [Serializable]
    public class StaticPackedAsset : IJSONPackable
    {
        [SerializeField]
        private string _title;

        [SerializeField]
        private StaticCrystAsset _mainAsset;

        [SerializeField]
        private List<StaticPackedAsset> _subAssets;

        public string Title => _title;

        public StaticCrystAsset MainAsset => _mainAsset;

        public List<StaticPackedAsset> SubAssets => _subAssets;

        public StaticPackedAsset() { }

        public StaticPackedAsset(string title, StaticCrystAsset mainAsset)
        { 
            _title = title;
            _mainAsset = mainAsset;
            _subAssets = new List<StaticPackedAsset>();
        }

        public StaticPackedAsset(string title, List<StaticPackedAsset> subAssets)
        {
            _title = title;
            _mainAsset = null;
            _subAssets = subAssets;
        }

        public void Pack(JSONPacker packer, JObject json)
        {
            json.Add("title", _title);

            if (_mainAsset != null)
            {
                json.Add("mainAsset", _mainAsset.AssetGUID);
            }

            var subAssets = new JArray();
            foreach (var asset in _subAssets)
            {
                JObject assetJObject = new();
                asset.Pack(packer, assetJObject);
                subAssets.Add(assetJObject);
            }

            json.Add("subAssets", subAssets);
        }

        public void Unpack(JSONUnpacker unpacker, JToken token)
        {
            var json = (JObject)token;

            if (json.TryGetValue("title", out var title))
            {
                _title = title.ToString();
            }

            if (json.TryGetValue("mainAsset", out var mainAsset))
            {
                _mainAsset = new StaticCrystAsset(mainAsset.ToString());
            }

            _subAssets = new List<StaticPackedAsset>();

            if (json.TryGetValue("subAssets", out var subAssets))
            {
                foreach (var asset in subAssets)
                {
                    var subAsset = new StaticPackedAsset();
                    subAsset.Unpack(unpacker, asset);

                    _subAssets.Add(subAsset);
                }
            }
        }
    }
}
