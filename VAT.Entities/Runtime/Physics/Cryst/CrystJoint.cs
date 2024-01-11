using System;
using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

using VAT.Shared;
using VAT.Shared.Utilities;

namespace VAT.Entities
{
    [Serializable]
    public struct CrystJointDrive {
        public float positionSpring;
        public float positionDamper;
        public float maximumForce;
    }

    /// <summary>
    /// The abstraction of a physics joint in Crystalline. When adding through code, invoke <see cref="CreateItem"/>.
    /// </summary>
    public abstract class CrystJoint : CachedMonoBehaviour, IRecreatable, ICrystJoint, IEntityComponent {
        public static ComponentCache<CrystJoint> Cache = new();

        [SerializeField]
        [HideInInspector]
        protected bool _hasJoint = false;
        public bool HasJoint { get { return _hasJoint; } }

        protected ICrystEntity _entity = null;
        public ICrystEntity Entity { get { return _entity; } }

        private bool _hasEntity = false;
        public bool HasEntity => _hasEntity;

        public abstract CrystBody Body { get; }
        public abstract CrystBody ConnectedBody { get; set; }

        public abstract CrystJointSpace JointSpace { get; }

        private void Awake() {
            Cache.Add(GameObject, this);

            OnJointAwake();
        }

        private void OnDestroy() {
            Cache.Remove(GameObject);

            OnJointDestroy();
        }

        private void OnEnable()
        {
            if (!HasEntity)
            {
                _entity = GetComponentInParent<ICrystEntity>();

                if (_entity != null)
                {
                    _hasEntity = true;
                    _entity.Hierarchy.AddJoint(this);
                }
            }
        }

        private void OnDisable()
        {
            if (HasEntity)
            {
                _hasEntity = false;

                _entity.Hierarchy.RemoveJoint(this);
                _entity = null;
            }
        }

        protected virtual void OnJointAwake() { }

        protected virtual void OnJointDestroy() { }

        /// <summary>
        /// Recreates the joint if it does not currently exist.
        /// </summary>
        public abstract void CreateItem();

        /// <summary>
        /// Destroys the joint.
        /// </summary>
        public abstract void DestroyItem();

        /// <summary>
        /// Recalculates the JointSpace for use in Joint target values.
        /// </summary>
        public abstract void RecalculateJointSpace();
    }
}
