using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Props
{
    public class GunBarrel : MonoBehaviour
    {
        [SerializeField]
        private Transform _firePoint = null;

        [SerializeField]
        private Chamber _chamber = null;

        public event Action OnFire;

        public Chamber Chamber
        {
            get
            {
                return _chamber;
            }
            set
            {
                _chamber = value;
            }
        }

        public SimpleTransform GetFirePoint()
        {
            return SimpleTransform.Create(_firePoint.position, _firePoint.rotation);
        }

        public void Fire()
        {
            if (Chamber.Cartridge != null)
            {
                OnFire?.Invoke();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Chamber == null)
            {
                GUIContent warningContent = new(EditorGUIUtility.IconContent("console.warnicon"));
                var style = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter
                };
                Handles.Label(transform.position, warningContent, style);

                Gizmos.color = Color.clear;
                Gizmos.DrawSphere(transform.position, 0.01f);
            }
        }
#endif
    }
}
