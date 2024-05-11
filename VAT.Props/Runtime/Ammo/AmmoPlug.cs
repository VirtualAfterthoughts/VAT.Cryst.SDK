using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Interaction;
using VAT.Interaction.Attachments;

namespace VAT.Props.Ammo
{
    public class AmmoPlug : Plug
    {
        private ConfigurableJoint _insertJoint = null;

        public void OnTriggerEnter(Collider other)
        {
            if (!IsInserted && other.TryGetComponent<AmmoSocket>(out var socket) && !socket.IsLocked)
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

                    float force = 1f - Mathf.Clamp01(dot * 5f);

                    float currSpring = _insertJoint.slerpDrive.positionSpring;
                    float newSpring = Mathf.Lerp(currSpring, 5000f * force, Time.deltaTime * 6f);
                    _insertJoint.slerpDrive = new JointDrive() { positionSpring = newSpring, positionDamper = 100f * (force + 0.05f), maximumForce = float.PositiveInfinity };

                    if (Vector3.Distance(insidePoint.position, transform.position) < 0.05f)
                    {
                        CompleteInsert();
                    }
                    else if (dot > 0f && Vector3.Distance(outsidePoint.position, transform.position) > distance * 0.5f)
                    {
                        CompleteEject();
                    }
                    break;
            }
        }

        protected override void OnBeginEject(Socket socket)
        {
            
        }

        protected override void OnBeginInsert(Socket socket)
        {
            socket.Host.ConnectHosts(new InteractableHostGroup(Host));

            var ammoSocket = socket as AmmoSocket;
            var outsidePoint = ammoSocket.OutsidePoint;

            var startRotation = Host.transform.rotation;
            Host.transform.rotation = socket.Host.transform.rotation;

            _insertJoint = Host.GetRigidbodyOrDefault().gameObject.AddComponent<ConfigurableJoint>();
            
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
            _insertJoint.connectedBody = socket.Host.GetRigidbodyOrDefault();

            _insertJoint.enableCollision = true;

            Host.transform.rotation = startRotation;
        }

        protected override void OnCompleteEject(Socket socket)
        {
            socket.Host.DisconnectHosts(new InteractableHostGroup(Host));

            Destroy(_insertJoint);
            _insertJoint = null;
        }

        protected override void OnCompleteInsert(Socket socket)
        {
            var ammoSocket = socket as AmmoSocket;
            var insidePoint = ammoSocket.InsidePoint;

            _insertJoint.connectedAnchor = socket.Host.transform.InverseTransformPoint(insidePoint.position);
            _insertJoint.yMotion = ConfigurableJointMotion.Locked;

            _insertJoint.angularXMotion = _insertJoint.angularYMotion = _insertJoint.angularZMotion = ConfigurableJointMotion.Locked;

            Host.DisableInteraction();
        }
    }
}
