using VAT.Input.Data;

using VAT.Input.Haptic;

namespace VAT.Input
{
    public interface IInputController
    {
        float GetGripForce()
        {
            float force = 0f;

            var trigger = GetTrigger();
            if (trigger != null)
            {
                force += trigger.GetForce() * 0.25f;
            }

            var grip = GetGrip();
            if (grip != null)
            {
                force += grip.GetForce() * 0.75f;
            }

            return force;
        }

        bool HasForceSensor();

        IInputHaptor GetHaptor();

        IInputTrigger GetTrigger();

        IInputTrigger GetGrip();

        IInputTrackpad GetThumbstick();

        IInputTrackpad GetTrackpad();

        IInputButton GetPrimaryButton();

        IInputButton GetSecondaryButton();

        HandPoseData GetHandPose();

        HandActions GetActions();
    }
}
