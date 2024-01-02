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

        [SerializeField]
        private List<StaticCrystChunk> _chunkScenes;

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

        public List<StaticCrystChunk> Chunks { 
            get {
                _chunkScenes ??= new();

                return _chunkScenes; 
            } 
        }

        public bool TryGetChunk(string name, out StaticCrystChunk chunk)
        {
            if (Chunks != null)
            {
                foreach (var item in Chunks)
                {
                    if (item.ChunkName == name)
                    {
                        chunk = item;
                        return true;
                    }
                }
            }

            chunk = null;
            return false;
        }

        public override List<StaticPackedAsset> CollectPackedAssets()
        {
            var packedAssets = new List<StaticPackedAsset>();

            var chunkAssets = new List<StaticPackedAsset>();
            foreach (var chunk in Chunks)
            {
                chunkAssets.Add(new StaticPackedAsset(chunk.ChunkName, chunk.Scene));
            }

            packedAssets.Add(new StaticPackedAsset("Chunks", chunkAssets));

            return packedAssets;
        }

        protected override void OnUnpackPackedAssets(List<StaticPackedAsset> packedAssets)
        {
            foreach (var packedAsset in packedAssets)
            {
                switch (packedAsset.Title)
                {
                    case "Chunks":
                        _chunkScenes = new List<StaticCrystChunk>();

                        foreach (var chunk in packedAsset.SubAssets)
                        {
                            _chunkScenes.Add(new StaticCrystChunk(chunk.Title, new StaticCrystScene(chunk.MainAsset.AssetGUID)));
                        }
                        break;
                }
            }
        }

#if UNITY_EDITOR
        public override string EditorAssetGroup => "Level";

        public override Type EditorAssetType => typeof(SceneAsset);

        protected override void OnValidateAssets(bool isBuilding = false)
        {
            base.OnValidateAssets(isBuilding);

            if (_chunkScenes != null)
            {
                foreach (var chunk in _chunkScenes)
                {
                    if (string.IsNullOrWhiteSpace(chunk.ChunkName))
                    {
                        chunk.GenerateName();
                        EditorUtility.SetDirty(this);
                    }

                    ValidateAsset(chunk.Scene, Address.BuildAddress(Address, chunk.ChunkName), isBuilding);
                }
            }
        }
#endif
    }
}
