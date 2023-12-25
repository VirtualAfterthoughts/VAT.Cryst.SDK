#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using VAT.Shared;
using VAT.Shared.Extensions;

namespace VAT.Zones
{
    public partial class SceneZone : CachedMonoBehaviour
    {
        private ZoneState _lastEditorState = ZoneState.NONE;
        private ZoneState EditorState
        {
            get
            {
                if (Application.isPlaying)
                {
                    return State;
                }
                else if (Camera.main != null)
                {
                    var position = Camera.main.transform.position;

                    if (Contains(position))
                    {
                        return ZoneState.PRIMARY;
                    }

                    foreach (var zone in _adjacentZones)
                    {
                        if (zone.Contains(position))
                        {
                            return ZoneState.SECONDARY;
                        }
                    }
                }

                return ZoneState.NONE;
            }
        }

        private void OnValidate()
        {
            Refresh();
        }

        private void OnUpdateEditorComponents()
        {
            var state = EditorState;
            if (_lastEditorState != state)
            {
                foreach (var component in _zoneComponents)
                {
                    bool currentHasFlag = (component.TriggerFlags & state) != 0;
                    bool previousHasFlag = (component.TriggerFlags & _lastEditorState) != 0;

                    if (currentHasFlag && !previousHasFlag)
                    {
                        component.OnEditorZoneEnabled();
                    }
                    else if (!currentHasFlag && previousHasFlag)
                    {
                        component.OnEditorZoneDisabled();
                    }
                }

                _lastEditorState = state;
            }
        }

        private void OnDrawGizmos()
        {
            Refresh();

            if (!Application.isPlaying)
            {
                OnUpdateEditorComponents();
            }

            Gizmos.color = Color.gray;

            foreach (var zone in _adjacentZones)
            {
                Gizmos.DrawLine(Center, zone.Center);
            }

            switch (EditorState)
            {
                case ZoneState.PRIMARY:
                    Gizmos.color = Color.green;
                    break;
                case ZoneState.SECONDARY:
                    Gizmos.color = new Color(1.0f, 0.64f, 0.0f);
                    break;
            }

            using (TempGizmoMatrix.Create()) {
                Gizmos.matrix = Transform.localToWorldMatrix;
                Gizmos.DrawWireCube(_zoneCollider.center, _zoneCollider.size);
            }

            Gizmos.DrawSphere(Center, 0.2f);
        }

        [MenuItem("GameObject/Crystalline/Zones/Scene Zone")]
        private static void MenuCreateItem(MenuCommand menuCommand)
        {
            GameObject go = new("Scene Zone", typeof(BoxCollider), typeof(SceneZone));
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

            Undo.RegisterCreatedObjectUndo(go, "Create Scene Zone");
        }
    }
}
#endif