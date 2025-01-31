using Newtonsoft.Json.Linq;

using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using VAT.Cryst.Game;
using VAT.Cryst.Utilities;

using VAT.Serialization.JSON;

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
        protected StaticCrystAssetT<Texture2D> _previewIcon;

        [SerializeField]
        protected Bounds _bounds;

        public Bounds Bounds => _bounds;

        public IWeakAssetT<Mesh> PreviewMesh => _previewMesh;

        public IWeakAssetT<Texture2D> PreviewIcon => _previewIcon;

        protected override void OnPack(JSONPacker packer, JObject json)
        {
            base.OnPack(packer, json);

            var boundsProperty = new JProperty("bounds", new JObject
                {
                    {
                        "center", new JObject
                        {
                            {"x", Bounds.center.x},
                            {"y", Bounds.center.y},
                            {"z", Bounds.center.z}
                        }
                    },
                    {
                        "extents", new JObject
                        {
                            {"x", Bounds.extents.x},
                            {"y", Bounds.extents.y},
                            {"z", Bounds.extents.z}
                        }
                    }
                }
            );

            json.Add(boundsProperty);
        }

        protected override void OnUnpack(JSONUnpacker unpacker, JObject json)
        {
            base.OnUnpack(unpacker, json);

            if (json.TryGetValue("bounds", out JToken boundsToken))
            {
                _bounds = boundsToken.ToObject<Bounds>();
            }
        }

        public override List<StaticPackedAsset> CollectPackedAssets()
        {
            var list = base.CollectPackedAssets();

            list.Add(new StaticPackedAsset("PreviewMesh", _previewMesh));
            list.Add(new StaticPackedAsset("PreviewIcon", _previewIcon));

            return list;
        }

        protected override void OnUnpackPackedAssets(List<StaticPackedAsset> packedAssets)
        {
            foreach (var packedAsset in packedAssets)
            {
                switch (packedAsset.Title)
                {
                    case "PreviewMesh":
                        _previewMesh = new StaticCrystAssetT<Mesh>(packedAsset.MainAsset.AssetGUID);
                        break;
                    case "PreviewIcon":
                        _previewIcon = new StaticCrystAssetT<Texture2D>(packedAsset.MainAsset.AssetGUID);
                        break;
                }
            }

            base.OnUnpackPackedAssets(packedAssets);
        }

        public override void GeneratePackedAssets(bool isBuilding = false)
        {
            GenerateBounds();

            GeneratePreviewMesh();

            GeneratePreviewIcon();

            base.GeneratePackedAssets(isBuilding);
        }

#if UNITY_EDITOR
        protected override void OnValidateAssets(bool isBuilding = false)
        {
            ValidateAsset(_previewMesh, new(Address.BuildAddress(Address.ID, "PreviewMesh")), isBuilding);

            ValidateAsset(_previewIcon, new(Address.BuildAddress(Address.ID, "PreviewIcon")), isBuilding);

            base.OnValidateAssets(isBuilding);
        }
#endif

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

        private void GeneratePreviewIcon()
        {
#if UNITY_EDITOR
            if (MainGameObject.EditorAssetT == null)
            {
                return;
            }

            EditorUtility.SetDirty(MainGameObject.EditorAssetT);
            var icon = AssetPreview.GetAssetPreview(MainGameObject.EditorAssetT);

            if (icon == null)
            {
                return;
            }

            var newIcon = new Texture2D(icon.width, icon.height, icon.format, false);

            newIcon.SetPixels32(icon.GetPixels32());

            var folderPath = CrystAssetManager.GetCrystRelativePath($"Packed Assets/Preview Icons/{StaticCrystal.CrystalInfo.Title}");
            CrystAssetManager.EnsureCrystFolderExists(folderPath);

            string path = folderPath + $"/{ShardInfo.Title} PreviewIcon.asset";

            if (AssetDatabase.LoadAssetAtPath<Texture2D>(path) != null)
            {
                AssetDatabase.DeleteAsset(path);
            }

            AssetDatabase.CreateAsset(newIcon, path);
            var textureAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            _previewIcon = new StaticCrystAssetT<Texture2D>();
            _previewIcon.ValidateGUID(textureAsset);
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

                foreach (var renderer in instance.GetComponentsInChildren<Renderer>())
                {
                    newBounds.Encapsulate(renderer.bounds);
                }

                _bounds = newBounds;
            }
#endif
        }
    }
}
