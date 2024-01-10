using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Entities.PhysX
{
    [RequireComponent(typeof(CrystRigidbody))]
    public sealed class CrystConfigurableJoint : CrystJoint {
        [SerializeField] private ConfigurableJoint _joint;
        [SerializeField] private CrystRigidbody _connectedBody;
        [SerializeField] private SimpleConfigurableJoint _info = SimpleConfigurableJoint.Default;

        private CrystRigidbody _body;

        private CrystConfigurableJointSpace _jointSpace;

        public ConfigurableJoint ConfigurableJoint { get { return _joint; } }

        public CrystRigidbody Rigidbody { get { return _body; } }
        public CrystRigidbody ConnectedRigidbody { 
            get  { 
                return _connectedBody; 
            } 
            set {
                _connectedBody = value;

                if (HasJoint)
                    _joint.connectedBody = value;
            }
        }

        public override CrystBody Body { get { return _body; } }
        public override CrystBody ConnectedBody { 
            get { 
                return _connectedBody;
            } 
            set {
                if (value is CrystRigidbody rigidbody)
                    ConnectedRigidbody = rigidbody;
            }
        }

        public override CrystJointSpace JointSpace { get { return _jointSpace; } }

        protected override void OnJointAwake() {
            base.OnJointAwake();

            _body = GameObject.AddOrGetComponent<CrystRigidbody>();

            if (_joint != null) {
                _hasJoint = true;
            }
        }

        [ContextMenu("Create ConfigurableJoint")]
        public override void CreateItem()
        {
            if (!HasJoint) {
                // Make sure there is a rigidbody
                _body.CreateItem();

                // Create the joint and apply settings
                _joint = GameObject.AddComponent<ConfigurableJoint>();
                _info.Apply(_joint);

                _hasJoint = true;
            }
        }

        [ContextMenu("Destroy ConfigurableJoint")]
        public override void DestroyItem()
        {
            if (HasJoint) {
                _info = SimpleConfigurableJoint.Create(_joint);

#if UNITY_EDITOR
                DestroyImmediate(_joint);
#else
                Destroy(_joint);
#endif

                // Make sure the process succeeded, it could have failed to delete
                if (!_joint) {
                    _joint = null;

                    _hasJoint = false;
                }
            }
        }

        public override void RecalculateJointSpace() {
            _jointSpace = new CrystConfigurableJointSpace(_joint);
        }

#if UNITY_EDITOR
        private void Reset() {
            _info = SimpleConfigurableJoint.Default;
            CreateItem();
        }

        private void OnValidate() {
            _hasJoint = _joint != null;

            if (_hasJoint) {
                _info.Apply(_joint);
            }
        }
#endif
    }
}
