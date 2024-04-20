using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Input;

namespace VAT.Characters
{
    public class UIRig : CrystRig
    {
        public GameObject uiCanvas;

        public override void OnRigEnable()
        {
            base.OnRigEnable();

            uiCanvas.transform.localScale = Vector3.zero;
        }

        public override void OnLateUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);

            var behaviourRig = RigManager.GetRigOrNull<IBehaviourRig>();
            if (behaviourRig != null)
            {
                var root = behaviourRig.GetRoot();
                transform.SetPositionAndRotation(root.position, root.rotation);
            }

            behaviourRig.TryGetArm(Handedness.RIGHT, out var arm);
            var secondaryButton = arm?.GetHandOrNull()?.GetInputControllerOrNull().GetActionsOrNull()?.SecondaryAction.State;

            if (secondaryButton != null && secondaryButton.Value)
            {
                uiCanvas.transform.localScale = Vector3.Slerp(uiCanvas.transform.localScale, Vector3.one, Time.deltaTime * 24f);
            }
            else
            {
                uiCanvas.transform.localScale = Vector3.Slerp(uiCanvas.transform.localScale, Vector3.zero, Time.deltaTime * 24f);
            }
        }
    }
}
