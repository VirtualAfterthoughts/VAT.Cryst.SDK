using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Packaging;
using VAT.Packaging.Preview;
using VAT.Pooling;
using VAT.Props;

namespace VAT.Interaction
{
    public class SpawnGun : MonoBehaviour
    {
        public Transform firePoint;
        public Grip triggerGrip;
        public PreviewMeshRenderer previewMeshRenderer;

        private IInteractor _mainInteractor = null;

        private SpawnableShardReference _selectedSpawnable = null;

        private void OnEnable()
        {
            previewMeshRenderer.transform.parent = null;

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
                    previewMeshRenderer.SetShard(shard);
                    previewMeshRenderer.Show();
                }
            }
            else
            {
                previewMeshRenderer.Hide();
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
                var controller = _mainInteractor.GetInputHand().GetInputController();
                var trigger = controller.GetTrigger();
                var axis = (trigger?.GetAxis()).GetValueOrDefault();

                bool pressing = axis > 0.7f;

                if (SpawnRaycast(out var point))
                {
                    previewMeshRenderer.transform.SetPositionAndRotation(point, Quaternion.identity);
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

                GlobalSpawner.Register(spawnable);

                GlobalSpawner.Spawn(new GlobalSpawner.SpawnRequestInfo()
                {
                    spawnable = spawnable,
                    position = point,
                });
            }
        }
    }
}
