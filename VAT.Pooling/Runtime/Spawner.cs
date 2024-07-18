using UnityEngine;

using VAT.Shared.Extensions;
using VAT.Shared;
using VAT.Packaging;
using VAT.Shared.Data;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VAT.Pooling
{
    public sealed class Spawner : MonoBehaviour, ITriggerable
    {
        [SerializeField]
        [Tooltip("The spawnable to place.")]
        private Spawnable _spawnable;

        [SerializeField]
        [Tooltip("An event called when the spawnable is placed.")]
        private SpawnableEvent placeEvent;

        [SerializeField]
        [Tooltip("Leave false if this spawnable should be placed when the level loads. If you set this to true, manually call Trigger to place the spawnable.")]
        private bool _manualSpawning = false;

        [SerializeField]
        [Tooltip("Should the spawned object use the scale of the spawnable placer?")]
        private bool _useScale = false;

        private void Awake()
        {
            AssetSpawner.Register(_spawnable);

            if (!_manualSpawning)
                Trigger();
        }

#if UNITY_EDITOR
        [ContextMenu("Trigger Spawn")]
#endif
        public void Trigger()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            Vector3? scale = _useScale ? transform.lossyScale : null;
            var info = new AssetSpawner.SpawnRequestInfo()
            {
                position = transform.position,
                rotation = transform.rotation,
                scale = scale,
                spawnable = _spawnable,
                spawnCallback = OnPlace,
            };

            AssetSpawner.Spawn(info);
        }

        private void OnPlace(AssetSpawner.SpawnCallbackInfo info)
        {
            placeEvent.Invoke(info.assetPoolable.gameObject, this);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying || _spawnable.shardReference == null)
                return;

            var address = _spawnable.shardReference.Address;

            if (_spawnable.shardReference.TryGetShard(out var shard))
            {
                this.name = $"Spawner ({shard.ShardInfo.Title})";
            }
            else if (address != Address.EMPTY)
            {
                this.name = $"Spawner ({address})";
            }
            else
            {
                this.name = "Spawner (Unknown)";
            }
        }

        private void OnDrawGizmos()
        {
            // Draw spawnable asset
            if (_spawnable.shardReference != null && _spawnable.shardReference.TryGetShard(out var shard))
            {
                var scale = _useScale ? this.transform.lossyScale : Vector3.one;

                SimpleTransform transform = SimpleTransform.Create(this.transform.position, this.transform.rotation, scale);

                var bounds = shard.Bounds;
                Gizmos.matrix = transform.localToWorldMatrix;

                Gizmos.color = new Color(1f, 0f, 1f);
                Gizmos.DrawMesh(shard.PreviewMesh?.EditorAssetT);

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(bounds.center, bounds.size);

                return;
            }

            // If the spawnable asset is never drawn, draw a question mark
            var questionMark = Resources.Load<GameObject>("Question Mark");
            if (questionMark != null)
            {
                questionMark.DrawGameObject(transform, Color.red, false);
            }
            else
            {
                Debug.LogError("Missing Cryst editor asset: \"Resources/Question Mark\"");
            }
        }

        [MenuItem("GameObject/Crystalline/Pooling/Spawner")]
        private static void MenuCreateItem(MenuCommand menuCommand)
        {
            GameObject go = new("Spawner", typeof(Spawner));
            go.transform.localScale = Vector3.one;

            if (menuCommand.context == null && SceneView.GetAllSceneCameras().Length > 0)
            {
                var camera = SceneView.GetAllSceneCameras()[0].transform;
                go.transform.position = camera.position + camera.forward * 10f;
            }
            else
            {
                GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            }

            Selection.activeObject = go;

            Undo.RegisterCreatedObjectUndo(go, "Create Spawner");
        }
#endif
    }
}
