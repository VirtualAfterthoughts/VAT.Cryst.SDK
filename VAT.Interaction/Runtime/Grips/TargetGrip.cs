using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public class TargetGrip : Grip
    {
        [Header("Target Grip")]
        [SerializeField]
        private Transform _target = null;

        [SerializeField]
        private Quaternion _symmetricalRotationOffset = Quaternion.identity;

        [SerializeField]
        [Min(0f)]
        private float _radius = 0f;

        public float Radius { get { return _radius; } set {  _radius = value; } }

        public Transform GetTargetTransform() 
        { 
            return _target; 
        }

        public virtual float GetWorldRadius()
        {
            return transform.lossyScale.Maximum() * _radius;
        }

        private void Awake()
        {
            if (_target == null)
            {
                _target = transform;
            }
        }

        public Quaternion GetAxisOffset(IGrabberPoint grabberPoint)
        {
            var grabPoint = grabberPoint.GetDefaultGrabPoint();
            var normal = grabberPoint.GetGrabNormal();

            float dot = Vector3.Dot(grabPoint.right, normal);

            var offset = _symmetricalRotationOffset;
            if (dot < 0f)
                offset = Quaternion.Inverse(offset);

            return offset;
        }

        public override SimpleTransform GetTargetInWorld(IGrabberPoint grabberPoint)
        {
            var target = GetTargetTransform();
            var grabPoint = grabberPoint.GetDefaultGrabPoint();
            var normal = grabberPoint.GetGrabNormal();

            var rotation = target.rotation * GetAxisOffset(grabberPoint);
            normal = rotation * (Quaternion.Inverse(grabPoint.rotation) * normal);

            return SimpleTransform.Create(target.position, rotation);
        }

        public override SimpleTransform GetGrabPoint(IInteractor interactor)
        {
            var grabPoint = base.GetGrabPoint(interactor);
            grabPoint.position -= (float3)interactor.GetRigidbody().transform.up * GetWorldRadius();
            return grabPoint;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (UnityEditor.Selection.activeGameObject != gameObject)
            {
                return;
            }
#endif

            Transform target = _target ? _target : transform;

            var worldRadius = GetWorldRadius();

            Gizmos.DrawWireSphere(target.position, worldRadius);

            var offset = target.rotation * _symmetricalRotationOffset;
            var axis = offset * Vector3.forward;
            var secondaryAxis = offset * Vector3.up;

            using (var color = TempGizmoColor.Create())
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(target.position, target.position + axis * worldRadius * 2f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(target.position, target.position + secondaryAxis * worldRadius * 2f);
            }

            if (DefaultClosedPose != null && DefaultClosedPose.previewMesh != null)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawMesh(DefaultClosedPose.previewMesh, 0, target.position + offset * Vector3.right * GetWorldRadius(), offset);

                Gizmos.color = new Color(1f, 1f, 1f, 0.005f);
                Gizmos.DrawWireMesh(DefaultClosedPose.previewMesh, 0, target.position + offset * Vector3.right * GetWorldRadius(), offset);
            } 
        }
    }
}
