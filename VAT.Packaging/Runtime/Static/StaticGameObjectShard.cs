using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VAT.Cryst.Game;
using VAT.Cryst.Utilities;

namespace VAT.Packaging
{
    public class StaticGameObjectShard : StaticShardT<GameObject>, IGameObjectShard
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

        [SerializeField]
        protected StaticCrystAssetT<Mesh> _previewMesh;

        [SerializeField]
        protected Bounds _bounds;

        public Bounds Bounds => _bounds;

        public IWeakAssetT<Mesh> PreviewMesh => _previewMesh;

        public override void GeneratePackedAssets(bool isBuilding = false)
        {
            GenerateBounds();

            GeneratePreviewMesh();

            base.GeneratePackedAssets(isBuilding);
        }

        protected override void OnValidateAssets(bool isBuilding = false)
        {
            ValidateAsset(_previewMesh, Address.BuildAddress(Address, "PreviewMesh"), isBuilding);

            base.OnValidateAssets(isBuilding);
        }

        private void GeneratePreviewMesh()
        {
#if UNITY_EDITOR
            if (MainGameObject.EditorAssetT == null)
            {
                return;
            }

            var newMesh = MeshSimplifier.Simplify(MainGameObject.EditorAssetT);

            var folderPath = CrystAssetManager.GetCrystRelativePath($"Packed Assets/Preview Meshes/{StaticCrystal.CrystalInfo.Title}");
            CrystAssetManager.EnsureCrystFolderExists(folderPath);
            
            string path = folderPath + $"/{ShardInfo.Title} PreviewMesh.mesh";
            AssetDatabase.CreateAsset(newMesh, path);
            var meshAsset = AssetDatabase.LoadAssetAtPath<Mesh>(path);

            _previewMesh = new StaticCrystAssetT<Mesh>();
            _previewMesh.ValidateGUID(meshAsset);
#endif
        }

        private void GenerateBounds()
        {
#if UNITY_EDITOR
            if (MainGameObject.EditorAssetT != null)
            {
                var editorAsset = MainGameObject.EditorAssetT;
                using var tempScene = TempGameObjectScene.Create(editorAsset, out var instance);
                
                var newBounds = new Bounds();
                
                foreach (var collider in instance.GetComponentsInChildren<Collider>())
                {
                    newBounds.Encapsulate(collider.bounds);
                }
                
                _bounds = newBounds;
            }
#endif
        }
    }
}
