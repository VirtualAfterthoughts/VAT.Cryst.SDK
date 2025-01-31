using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Misc
{
    [RequireComponent(typeof(LineRenderer))]
    public abstract class LineDrawer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Should the line update every frame, or only once?")]
        private bool _renderOnUpdate = true;

        private LineRenderer _renderer = null;

        private void OnEnable()
        {
            GetLine();

            if (!_renderOnUpdate)
            {
                RenderLine();
            }
        }

        private void GetLine()
        {
            _renderer = GetComponent<LineRenderer>();
        }

        private void LateUpdate()
        {
            if (_renderOnUpdate)
            {
                RenderLine();
            }
        }

        public void RenderLine()
        {
            OnRenderLine(_renderer);
        }

        protected abstract void OnRenderLine(LineRenderer renderer);

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                return;
            }

            GetLine();

            RenderLine();
        }
#endif
    }
}
