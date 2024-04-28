using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging.Preview
{
    public class PreviewMeshRenderer : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter _previewMeshFilter = null;

        [SerializeField]
        private Transform _boundsTransform = null;

        private Mesh _cachedMesh = null;
        private Bounds _cachedBounds = default;

        private bool _isShown = false;

        private void OnEnable()
        {
            Hide();
        }

        public void Show()
        {
            if (_previewMeshFilter != null)
            {
                _previewMeshFilter.sharedMesh = _cachedMesh;
            }

            UpdateBounds(_cachedBounds.size, _cachedBounds.center);

            _isShown = true;
        }

        private void UpdateBounds(Vector3 size, Vector3 center)
        {
            if (_boundsTransform != null)
            {
                _boundsTransform.localScale = size;
                _boundsTransform.localPosition = center;
            }
        }

        public void Hide()
        {
            if (_previewMeshFilter != null)
            {
                _previewMeshFilter.sharedMesh = null;
            }

            UpdateBounds(Vector3.zero, Vector3.zero);


            _isShown = false;
        }

        public void SetShard(IGameObjectShard shard)
        {
            shard.PreviewMesh?.LoadAsset((m) =>
            {
                SetMesh(m);
            });

            SetBounds(shard.Bounds);
        }

        public void SetBounds(Bounds bounds)
        {
            if (_isShown)
            {
                UpdateBounds(bounds.size, bounds.center);
            }

            _cachedBounds = bounds;
        }

        public void SetMesh(Mesh mesh)
        {
            if (_isShown && _previewMeshFilter != null)
            {
                _previewMeshFilter.sharedMesh = mesh;
            }

            _cachedMesh = mesh;
        }
    }
}
