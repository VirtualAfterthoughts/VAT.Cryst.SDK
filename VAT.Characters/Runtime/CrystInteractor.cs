using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Entities.PhysX;

using VAT.Input;
using VAT.Interaction;
using VAT.Shared.Data;

namespace VAT.Characters
{
    public class CrystInteractor : Interactor
    {
        public CrystRigidbody rb;
        public Handedness handedness;
        public IInputController controller;
        public SimpleTransform palm = SimpleTransform.Default;

        private void Start()
        {
            rb = GetComponent<CrystRigidbody>();
        }

        public override GameObject GetGrabberGameObject()
        {
            return rb.gameObject;
        }

        public override Rigidbody GetGrabberRigidbody()
        {
            return rb.Rigidbody;
        }

        public override SimpleTransform GetGrabPoint()
        {
            return SimpleTransform.Create(transform).Transform(palm);
        }

        public void LateUpdate()
        {
            controller.TryGetGrip(out var grip);

            if (grip.GetAxis() > 0.6f && AttachedGrippable == null && HoveringInteractable != null && HoveringInteractable.CanAttach(this))
            {
                AttachGrippable(HoveringInteractable);
            }
            else if (grip.GetAxis() < 0.4f && AttachedGrippable != null)
            {
                DetachGrippable(AttachedGrippable);
            }
        }

        protected override void OnAttachGrippable(IGrippable grippable)
        {
            grippable.CreateJoint(this);
        }

        protected override void OnDetachGrippable(IGrippable grippable)
        {
            grippable.DestroyJoint(this);
        }

        protected override void OnUpdateAttachments()
        {
        }

        protected override void OnUpdateHover()
        {
            var colliders = Physics.OverlapCapsule(transform.position, transform.position + transform.forward * 2f, 0.2f, ~0, QueryTriggerInteraction.Collide);
            IInteractable interactable = null;

            foreach (var collider in colliders)
            {
                var go = collider.attachedRigidbody ? collider.attachedRigidbody.gameObject : collider.gameObject;

                if (IInteractable.Cache.TryGet(go, out var found) || IInteractable.Cache.TryGet(collider.gameObject, out found))
                {
                    interactable = found;
                }
            }

            HoveringInteractable = interactable;
        }
    }
}
