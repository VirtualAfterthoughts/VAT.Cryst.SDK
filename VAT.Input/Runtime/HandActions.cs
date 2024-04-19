using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Cryst.State;
using VAT.Input.Skeleton;

namespace VAT.Input
{
    public class HandActions
    {
        public BoolState GrabAction = new();

        public BoolState AbilityGrabAction = new();

        public BoolState PrimaryAction = new();

        public BoolState SecondaryAction = new();

        public void UpdateActions(IInputHand inputHand, IInputController inputController)
        {
            var blendPose = inputHand.GetHandPose();

            float maxCurl = 0f;

            foreach (var finger in blendPose.fingers)
            {
                maxCurl = Mathf.Max(maxCurl, finger.GetCurl());
            }

            float secondaryCurl = 0f;
            for (var i = 1; i < blendPose.fingers.Length; i++)
            {
                secondaryCurl = Mathf.Max(secondaryCurl, blendPose.fingers[i].GetCurl());
            }

            bool gripPose = maxCurl > 0.7f;
            bool interactPose = secondaryCurl > 0.7f && inputController.GetTriggerOrNull()?.GetAxis() > 0.7f;

            GrabAction.State = gripPose;
            AbilityGrabAction.State = interactPose;

            var primaryButton = inputController.GetPrimaryButtonOrNull()?.GetPressed();
            var secondaryButton = inputController.GetSecondaryButtonOrNull()?.GetPressed();

            PrimaryAction.State = primaryButton.GetValueOrDefault();
            SecondaryAction.State = secondaryButton.GetValueOrDefault();
        }
    }
}
