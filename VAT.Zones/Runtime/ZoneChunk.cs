using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

using VAT.Packaging;
using VAT.Scene;

namespace VAT.Zones
{
    public sealed class ZoneChunk : ZoneComponent
    {
        [SerializeField]
        private LevelChunkReference _chunkReference;

        private StaticCrystChunk _chunk;
        private bool _hasChunk = false;

        private void Awake()
        {
            AssetPackager.HookOnReady(OnReady);
        }

        private void OnReady()
        {
            _hasChunk = _chunkReference.TryGetChunk(out _chunk);
        }

        public override void OnZoneEnabled()
        {
            if (_hasChunk)
            {
                CrystSceneManager.SceneSession.LoadAdditive(_chunk.Scene).Forget();
            }
        }

        public override void OnZoneDisabled()
        {
            if (_hasChunk)
            {
                CrystSceneManager.SceneSession.UnloadAdditive(_chunk.Scene, false).Forget();
            }
        }

#if UNITY_EDITOR
        public override void OnEditorZoneEnabled()
        {
            if (_chunkReference.TryGetChunk(out var chunk) && chunk.Scene.EditorScene != null)
            {
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(chunk.Scene.EditorScene), OpenSceneMode.Additive);
            }
        }

        public override void OnEditorZoneDisabled()
        {
            if (_chunkReference.TryGetChunk(out var chunk) && chunk.Scene.EditorScene != null)
            {
                var path = AssetDatabase.GetAssetPath(chunk.Scene.EditorScene);
                EditorSceneManager.CloseScene(SceneManager.GetSceneByPath(path), true);
            }
        }
#endif
    }
}
