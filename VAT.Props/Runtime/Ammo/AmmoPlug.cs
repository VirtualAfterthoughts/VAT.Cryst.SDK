using UnityEngine;

using VAT.Interaction.Attachments;
using VAT.Shared.Data;

namespace VAT.Props.Ammo
{
    public class AmmoPlug : Plug
    {
        private ConfigurableJoint _insertJoint = null;
        private ConfigurableJointSpace _jointSpace = null;

        private void OnTriggerEnter(Collider other)
        {
            if (!IsInserted && other.TryGetComponent<AmmoSocket>(out var socket) && socket.CanInsert(this))
            {
                ConfirmInsert(socket);
            }
        }

        public void FixedUpdate()
        {
            var ammoSocket = InsertedSocket as AmmoSocket;

            switch (State)
            {
                case PlugState.EJECTING:
                case PlugState.INSERTING:
                    var insidePoint = ammoSocket.InsidePoint;
                    var outsidePoint = ammoSocket.OutsidePoint;
                    float distance = Vector3.Distance(insidePoint.position, outsidePoint.position);

                    var dot = Vector3.Dot((insidePoint.position - outsidePoint.position) / distance, (outsidePoint.position - transform.position) / distance);

                    if (dot < -0.7f && State != PlugState.EJECTING)
                    {
                        _insertJoint.yDrive = new JointDrive() { positionSpring = 50000f, positionDamper = 50f, maximumForce = 1000f };
                        var targetPos = _jointSpace.GetTargetPositionWorld(_insertJoint.transform.position);
                        targetPos.x = 0f;
                        targetPos.z = 0f;
                        _insertJoint.targetPosition = Vector3.MoveTowards(targetPos, _insertJoint.targetPosition, 0.001f);
                    }
                    else
                    {
                        _insertJoint.yDrive = new JointDrive() { positionDamper = 5f, maximumForce = 100f };
                        _insertJoint.targetPosition = Vector3.zero;
                    }

                    float force = 1f - Mathf.Clamp01(dot * 5f);

                    float currSpring = _insertJoint.slerpDrive.positionSpring;
                    float newSpring = Mathf.Lerp(currSpring, 5000f * force, Time.deltaTime * 6f);
                    _insertJoint.slerpDrive = new JointDrive() { positionSpring = newSpring, positionDamper = newSpring * 0.1f, maximumForce = float.PositiveInfinity };

                    float ejectDistance = State == PlugState.EJECTING ? 0.1f : 0.5f;

                    if (State != PlugState.EJECTING && dot <= -0.93f)
                    {
                        CompleteInsert();
                    }
                    else if (dot > 0f && Vector3.Distance(outsidePoint.position, transform.position) > distance * ejectDistance)
                    {
                        CompleteEject();
                    }
                    break;
            }
        }

        protected override void OnBeginEject(Socket socket)
        {
            FreeJoint(socket);
        }

        protected override void OnBeginInsert(Socket socket)
        {
            // Makes the joint less "clippy" when inserting.
            // Reset immediately after ejecting for performance.
            Host.GetRigidbody().solverIterations = 256;

            socket.Host.AttachGroup(Host.SelfGroup);

            var ammoSocket = socket as AmmoSocket;
            var outsidePoint = ammoSocket.OutsidePoint;

            var startRotation = Host.transform.rotation;
            Host.transform.rotation = socket.Host.transform.rotation;

            _insertJoint = Host.GetRigidbody().gameObject.AddComponent<ConfigurableJoint>();
            
            _insertJoint.xDrive = _insertJoint.zDrive = new JointDrive() { positionSpring = 5000000f, positionDamper = 10000f, maximumForce =  float.PositiveInfinity };
            
            var insideToOut = ammoSocket.InsidePoint.position - ammoSocket.OutsidePoint.position;
            _insertJoint.secondaryAxis = _insertJoint.transform.InverseTransformDirection(insideToOut.normalized);

            _insertJoint.xMotion = _insertJoint.yMotion = _insertJoint.zMotion = ConfigurableJointMotion.Limited;
            _insertJoint.linearLimit = new SoftJointLimit() { limit = insideToOut.magnitude };

            _insertJoint.yDrive = new JointDrive() { positionDamper = 10f, maximumForce = 10f };
            _insertJoint.rotationDriveMode = RotationDriveMode.Slerp;

            _insertJoint.autoConfigureConnectedAnchor = false;
            _insertJoint.anchor = _insertJoint.transform.InverseTransformPoint(transform.position);
            _insertJoint.connectedAnchor = socket.Host.transform.InverseTransformPoint(outsidePoint.position);
            _insertJoint.connectedBody = socket.Host.GetRigidbody();

            _insertJoint.enableCollision = true;

            Host.transform.rotation = startRotation;

            _jointSpace = new ConfigurableJointSpace(_insertJoint);
        }

        private void FreeJoint(Socket socket)
        {
            var ammoSocket = socket as AmmoSocket;
            var outsidePoint = ammoSocket.OutsidePoint;

            _insertJoint.connectedAnchor = socket.Host.transform.InverseTransformPoint(outsidePoint.position);

            _insertJoint.xMotion = _insertJoint.yMotion = _insertJoint.zMotion = ConfigurableJointMotion.Limited;
            _insertJoint.angularXMotion = _insertJoint.angularYMotion = _insertJoint.angularZMotion = ConfigurableJointMotion.Free;

            _insertJoint.projectionMode = JointProjectionMode.None;

            Host.EnableInteraction();
        }

        private void LockJoint(Socket socket)
        {
            var ammoSocket = socket as AmmoSocket;
            var insidePoint = ammoSocket.InsidePoint;

            _insertJoint.connectedAnchor = socket.Host.transform.InverseTransformPoint(insidePoint.position);

            _insertJoint.xMotion = _insertJoint.yMotion = _insertJoint.zMotion = ConfigurableJointMotion.Locked;

            _insertJoint.angularXMotion = _insertJoint.angularYMotion = _insertJoint.angularZMotion = ConfigurableJointMotion.Locked;

            _insertJoint.projectionMode = JointProjectionMode.PositionAndRotation;
            _insertJoint.projectionDistance = 0f;
            _insertJoint.projectionAngle = 0f;

            Host.DisableInteraction();
        }

        protected override void OnCompleteEject(Socket socket)
        {
            socket.Host.DetachGroup(Host.SelfGroup);

            Destroy(_insertJoint);
            _insertJoint = null;

            Host.GetRigidbody().solverIterations = Physics.defaultSolverIterations;
        }

        protected override void OnCompleteInsert(Socket socket)
        {
            LockJoint(socket);
        }
    }
}
