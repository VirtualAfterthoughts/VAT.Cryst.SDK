using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Avatars.Posing;
using VAT.Interaction;
using VAT.Shared.Data;

namespace VAT.Misc
{
    public struct GripPoint : IGrabberPoint
    {
        public HumanoidHandPoser poser;

        public GripPoint(HumanoidHandPoser poser)
        {
            this.poser = poser;
        }

        public SimpleTransform GetDefaultGrabPoint()
        {
            return GetGrabPoint(Vector2.up);
        }

        public SimpleTransform GetGrabCenter()
        {
            return GetDefaultGrabPoint();
        }

        public Vector3 GetGrabNormal()
        {
            return -poser.Hand.Hand.up;
        }

        public SimpleTransform GetGrabPoint(Vector2 position)
        {
            return poser.Hand.GetPointOnPalm(position);
        }

        public SimpleTransform GetParentTransform()
        {
            return poser.Hand.Hand.Transform;
        }
    }

    [ExecuteAlways]
    public class TestPoseGizmo : MonoBehaviour
    {
        public HumanoidHandPoser poser;
        public TargetGrip grip;

        private GripPoint _point;

        // Update is called once per frame
        void Update()
        {
            if (poser == null || grip == null) return;

            _point = new GripPoint(poser);

            var parentLocal = SimpleTransform.Create(poser.transform).InverseTransform(_point.GetParentTransform());

            var point = grip.GetTargetInWorld(_point);

            var worldInteractor = (SimpleTransform.Create(poser.transform).Transform(parentLocal).Transform(grip.GetTargetInInteractor(_point)));
            poser.transform.rotation = (point.rotation * Quaternion.Inverse(worldInteractor.rotation)) * poser.transform.rotation;

            worldInteractor = (SimpleTransform.Create(poser.transform).Transform(parentLocal).Transform(grip.GetTargetInInteractor(_point)));
            poser.transform.position = (Vector3)(point.position - worldInteractor.position) + poser.transform.position;
        }
    }
}
