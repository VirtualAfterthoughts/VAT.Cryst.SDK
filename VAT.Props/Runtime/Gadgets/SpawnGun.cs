using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Packaging;
using VAT.Pooling;
using VAT.Props;

namespace VAT.Interaction
{
    public class SpawnGun : MonoBehaviour
    {
        public Transform firePoint;
        public Grip triggerGrip;
        public MeshFilter previewMeshFilter;
        public Transform previewBounds;

        private IInteractor _mainInteractor = null;

        private SpawnableShardReference _selectedSpawnable = null;

        private void OnEnable()
        {
            previewMeshFilter.transform.parent = null;

            triggerGrip.OnAttached += OnTriggerGripAttached;
            triggerGrip.OnDetached += OnTriggerGripDetached;
        }

        private void OnDisable()
        {
            triggerGrip.OnAttached -= OnTriggerGripAttached;
            triggerGrip.OnDetached -= OnTriggerGripDetached;
        }

        private void OnTriggerGripAttached(IInteractor interactor)
        {
            if (_mainInteractor == null)
            {
                _mainInteractor = interactor;

                var module = interactor.GetModule<IInteractorSpawnerModule>();

                if (module != null)
                {
                    module.OnSpawnableSelected += OnSpawnableSelected;
                    module.SetSpawningActive(true);
                }
            }
        }

        private void OnTriggerGripDetached(IInteractor interactor)
        {
            if (_mainInteractor == interactor)
            {
                _mainInteractor = null;

                var module = interactor.GetModule<IInteractorSpawnerModule>();

                if (module != null)
                {
                    module.OnSpawnableSelected -= OnSpawnableSelected;
                    module.SetSpawningActive(false);
                }
            }
        }
        
        private void OnSpawnableSelected(SpawnableShardReference reference)
        {
            _selectedSpawnable = reference;
        }

        private bool _wasPressingTrigger = false;

        private void TogglePreviewMesh(bool show)
        {
            if (show)
            {
                if (_selectedSpawnable != null && _selectedSpawnable.TryGetShard(out var shard))
                {
                    shard.PreviewMesh?.LoadAsset((m) =>
                    {
                        previewMeshFilter.sharedMesh = m;
                    });

                    previewBounds.gameObject.SetActive(true);
                    previewBounds.transform.localScale = shard.Bounds.size;
                    previewBounds.transform.localPosition = shard.Bounds.center;
                }
            }
            else
            {
                previewMeshFilter.sharedMesh = null;
                previewBounds.gameObject.SetActive(false);
            }
        }

        private bool SpawnRaycast(out Vector3 point)
        {
            bool success = Physics.Raycast(firePoint.position, firePoint.forward, out var hitInfo, 10f, ~0, QueryTriggerInteraction.Ignore);
            point = default;

            if (success)
            {
                point = hitInfo.point;

                if (_selectedSpawnable != null && _selectedSpawnable.TryGetShard(out var shard))
                {
                    point += 0.5f * shard.Bounds.size.y * hitInfo.normal;
                }

                return true;
            }

            return false;
        }

        private void Update()
        {
            if (_mainInteractor != null)
            {
                var controller = _mainInteractor.GetHandOrNull().GetInputControllerOrNull();
                var trigger = controller.GetTriggerOrNull();
                var axis = (trigger?.GetAxis()).GetValueOrDefault();

                bool pressing = axis > 0.7f;

                if (SpawnRaycast(out var point))
                {
                    previewMeshFilter.transform.SetPositionAndRotation(point, Quaternion.identity);
                }

                // Trigger down
                if (pressing && !_wasPressingTrigger)
                {
                    TogglePreviewMesh(true);
                }
                // Trigger up
                else if (!pressing && _wasPressingTrigger)
                {
                    Fire();

                    TogglePreviewMesh(false);
                }

                _wasPressingTrigger = pressing;
            }
        }

        private void Fire()
        {
            if (_selectedSpawnable == null)
            {
                return;
            }

            if (SpawnRaycast(out var point))
            {
                var spawnable = new Spawnable(_selectedSpawnable);

                AssetSpawner.Register(spawnable);

                AssetSpawner.Spawn(new AssetSpawner.SpawnRequestInfo()
                {
                    spawnable = spawnable,
                    position = point,
                });
            }
        }
    }
}
