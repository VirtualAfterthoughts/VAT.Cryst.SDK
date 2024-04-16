using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Utilities;

namespace VAT.Input
{
    [RequireComponent(typeof(Collider))]
    public class UIPlane : MonoBehaviour
    {
        [SerializeField]
        private Vector2 center = Vector2.zero;

        [SerializeField]
        private Vector2 size = Vector2.one;

        public static ComponentCache<UIPlane> Cache = new();

        private void OnEnable()
        {
            Cache.Add(gameObject, this);
        }

        private void OnDisable()
        {
            Cache.Remove(gameObject, this);
        }

        public Plane GetPlane()
        {
            return new Plane(-transform.forward, GetCenter());
        }

        public Quaternion GetRotation()
        {
            return transform.rotation;
        }

        public Vector2 GetSize()
        {
            var scale = transform.lossyScale;
            return Vector2.Scale(size, new Vector2(scale.x, scale.y));
        }

        public Vector3 GetCenter()
        {
            return transform.TransformPoint(center);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;

            Gizmos.matrix = transform.localToWorldMatrix;
            
            Gizmos.DrawWireCube(center, size);
        }
    }
}
