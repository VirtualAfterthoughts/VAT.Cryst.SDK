using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Utilities;
using VAT.Shared;
using VAT.Shared.Extensions;
using VAT.Entities;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VAT.Zones
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider))]
    public partial class SceneZone : CachedMonoBehaviour
    {
        public static ComponentCache<SceneZone> Cache = new();

        private static readonly Collider[] _adjacentZoneCache = new Collider[64];

        [SerializeField]
        private BoxCollider _zoneCollider;

        [SerializeField]
        private EntityType _entityMask = EntityType.PLAYER;

        [SerializeField]
        private SceneZone[] _adjacentZones;

        [SerializeField]
        private ZoneComponent[] _zoneComponents;

        public BoxCollider ZoneCollider => _zoneCollider;

        public SceneZone[] AdjacentZones => _adjacentZones;

        public Vector3 Center => Transform.TransformPoint(_zoneCollider.center);

        public Vector3 Size => Vector3.Scale(Transform.lossyScale, _zoneCollider.size);

        private void Awake()
        {
            Cache.Add(gameObject, this);
        }

        private void OnDestroy()
        {
            Cache.Remove(gameObject, this);
        }

        private void OnEnable()
        {
            Refresh();
        }

        public bool Contains(Vector3 point)
        {
            return _zoneCollider.bounds.Contains(point);
        }

        public void Refresh()
        {
            RefreshCollider();
            RefreshAdjacentZones();
            RefreshZoneComponents();
        }

        private void RefreshZoneComponents()
        {
            _zoneComponents = GetComponentsInChildren<ZoneComponent>();
        }

        private void RefreshAdjacentZones()
        {
            var transform = this.transform;
            var center = transform.TransformPoint(_zoneCollider.center);
            var extents = Vector3.Scale(transform.lossyScale, _zoneCollider.size * 0.5f);

            var count = Physics.OverlapBoxNonAlloc(center, extents, _adjacentZoneCache, transform.rotation, ~0, QueryTriggerInteraction.Collide);

            List<SceneZone> zones = new();

            for (var i = 0; i < count; i++)
            {
                if (_adjacentZoneCache[i].TryGetComponent<SceneZone>(out var zone) && zone != this)
                {
                    zones.Add(zone);
                }
            }

            _adjacentZones = zones.ToArray();
        }

        private void RefreshCollider()
        {
            if (_zoneCollider == null)
            {
                _zoneCollider = GameObject.AddOrGetComponent<BoxCollider>();
            }

            _zoneCollider.isTrigger = true;
        }
    }
}
