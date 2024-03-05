using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using VAT.Avatars;
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
        [Min(0f)]
        private float _radius = 0f;

        public float Radius { get { return _radius; } set {  _radius = value; } }

        public Transform GetTargetTransform() 
        { 
            if (_target == null)
            {
                _target = transform;
            }

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

        public override SimpleTransform GetTargetInWorld(IGrabPoint point, HandPoseData pose)
        {
            var pivot = GetPivotInWorld(point, pose);
            var normalRelative = point.GetDefaultGrabPoint().InverseTransformDirection(point.GetGrabNormal());
            pivot.position -= pivot.TransformDirection(normalRelative) * GetWorldRadius();

            return pivot;
        }

        public override SimpleTransform GetPivotInWorld(IGrabPoint point, HandPoseData pose)
        {
            var target = GetTargetTransform();

            var rotation = target.rotation;

            return SimpleTransform.Create(target.position, rotation);
        }

        public override SimpleTransform GetPivotInInteractor(IGrabPoint point, HandPoseData pose)
        {
            var grabPoint = base.GetPivotInInteractor(point, pose);
            grabPoint.position += math.down() * GetWorldRadius();
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

            var rotationOffset = DefaultClosedPose ? DefaultClosedPose.data.rotationOffset.normalized : Quaternion.identity;
            var offset = target.rotation * rotationOffset;
            var axis = offset * Vector3.forward;
            var secondaryAxis = offset * Vector3.up;

            using (var color = TempGizmoColor.Create())
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(target.position, target.position + axis * worldRadius * 2f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(target.position, target.position + secondaryAxis * worldRadius * 2f);
            }
        }
    }
}
