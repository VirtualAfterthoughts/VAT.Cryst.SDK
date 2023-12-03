using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public abstract class Interactor : MonoBehaviour, IInteractor
    {
        private IInteractable _hoveringInteractable;
        public IInteractable HoveringInteractable
        {
            get
            {
                return _hoveringInteractable;
            }
            set
            {
                // Check if we are able to hover currently
                if (!CanHover())
                {
                    Debug.LogWarning("Attempted to hover an Interactable, but the Interactor cannot hover!", this);
                    value = null;
                }

                // Make sure this is a different interactable
                if (_hoveringInteractable != value)
                {
                    // Begin hover
                    if (_hoveringInteractable == null)
                    {
                        value.OnHoverBegin(this);
                    }
                    // End hover
                    else
                    {
                        _hoveringInteractable.OnHoverEnd(this);

                        if (value != null)
                            value.OnHoverBegin(this);
                    }

                    _hoveringInteractable = value;
                }
            }
        }

        private readonly List<IGrippable> _attachedGrippables = new();
        public List<IGrippable> AttachedGrippables => _attachedGrippables;

        public IGrippable AttachedGrippable => _attachedGrippables.FirstOrDefault();

        public bool HasAttachedGrippable => _attachedGrippables.Count > 0;

        public bool HasHoveringInteractable => HoveringInteractable != null;

        private void Awake()
        {
            IInteractor.Cache.Add(gameObject, this);

            OnAwake();
        }

        private void OnDestroy()
        {
            IInteractor.Cache.Remove(gameObject);
        }

        protected virtual void OnAwake() { }

        public void FixedUpdate()
        {
            UpdateHover();
            UpdateAttachments();
        }

        public void UpdateHover()
        {
            if (!CanHover())
                return;

            OnUpdateHover();
        }

        public void UpdateAttachments()
        {
            OnUpdateAttachments();
        }

        protected abstract void OnUpdateHover();

        protected abstract void OnUpdateAttachments();

        public virtual bool CanAttach()
        {
            return !HasAttachedGrippable;
        }

        public virtual bool CanHover()
        {
            return !HasAttachedGrippable;
        }

        public void AttachGrippable(IGrippable grippable)
        {
            if (!CanAttach())
            {
                Debug.LogWarning("Attempted to attach an interactable, but the Interactor cannot attach it!", this);
                return;
            }

            HoveringInteractable = null;

            _attachedGrippables.Add(grippable);

            OnAttachGrippable(grippable);
            grippable.OnAttached(this);

            grippable.OnForceDetachEvent += OnForceDetach;
        }

        protected abstract void OnAttachGrippable(IGrippable grippable);

        public void DetachGrippable(IGrippable grippable)
        {
            _attachedGrippables.Remove(grippable);

            OnDetachGrippable(grippable);
            grippable.OnDetached(this);

            grippable.OnForceDetachEvent -= OnForceDetach;
        }

        protected abstract void OnDetachGrippable(IGrippable grippable);

        public void DetachGrippables()
        {
            for (var i = _attachedGrippables.Count - 1; i >= 0; i--)
            {
                DetachGrippable(_attachedGrippables[i]);
            }
        }

        private void OnForceDetach(IGrabber grabber, IGrippable grippable)
        {
            if (grabber.Equals(this))
            {
                DetachGrippable(grippable);
            }
        }

        public abstract GameObject GetGrabberGameObject();
        public abstract Rigidbody GetGrabberRigidbody();
        public abstract SimpleTransform GetGrabPoint();
    }
}
