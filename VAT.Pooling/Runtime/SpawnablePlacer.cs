using System.Collections;
using System.Collections.Generic;

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
    public sealed class SpawnablePlacer : MonoBehaviour, ITriggerable
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
            AssetSpawner.Spawn(_spawnable, transform.position, transform.rotation, scale, OnPlace);
        }

        private void OnPlace(AssetPoolable poolable)
        {
            placeEvent.Invoke(poolable.gameObject, this);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying || _spawnable.contentReference == null)
                return;

            var address = _spawnable.contentReference.Address;

            if (_spawnable.contentReference.TryGetContent(out var content))
            {
                this.name = $"Spawnable Placer ({content.ContentInfo.Title})";
            }
            else if (address != Address.EMPTY)
            {
                this.name = $"Spawnable Placer ({address})";
            }
            else
            {
                this.name = "Spawnable Placer (Unknown)";
            }
        }

        private void OnDrawGizmos()
        {
            // Draw spawnable asset
            if (_spawnable.contentReference != null && _spawnable.contentReference.TryGetContent(out var content))
            {
                // Do we have an editor asset?
                // If so, draw the gizmo
                if (content.MainAssetT != null && content.MainAssetT.EditorAssetT != null)
                {
                    var go = content.MainAssetT.EditorAssetT;
                    SimpleTransform transform;

                    if (!_useScale)
                    {
                        transform = SimpleTransform.Create(this.transform.position, this.transform.rotation, go.transform.lossyScale);
                    }
                    else
                    {
                        transform = SimpleTransform.Create(this.transform);
                    }

                    go.DrawGameObject(transform, Color.green, false);
                    return;
                }
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

        [MenuItem("GameObject/Crystalline/Pooling/Spawnable Placer")]
        private static void MenuCreateItem(MenuCommand menuCommand)
        {
            GameObject go = new("Spawnable Placer", typeof(SpawnablePlacer));
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

            Undo.RegisterCreatedObjectUndo(go, "Create Spawnable Placer");
        }
#endif
    }
}
