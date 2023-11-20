using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Extensions;
using VAT.Shared;
using VAT.Packaging;

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
        [Tooltip("Leave false if this spawnable should be placed when the level loads. If you set this to true, manually call Trigger to place the spawnable.")]
        private bool _manualSpawning = false;

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

            AssetSpawner.Spawn(_spawnable, transform.position, transform.rotation, transform.lossyScale);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_spawnable.contentReference == null)
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

            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Selection.activeObject = go;

            Undo.RegisterCreatedObjectUndo(go, "Create Spawnable Placer");
        }
#endif
    }
}
