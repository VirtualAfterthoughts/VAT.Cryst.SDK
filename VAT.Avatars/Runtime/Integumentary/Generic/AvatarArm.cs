using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.REWORK;
using VAT.Avatars.Skeletal;

using VAT.Input;
using VAT.Shared.Data;

namespace VAT.Avatars.Integumentary {
    public struct AvatarArm {
        public Handedness Handedness { get; }
        public IBone DataRig { get; }
        public IBone PhysRig { get; }
        public IArmGroup DataArm { get; }
        public IArmGroup PhysArm { get; }

        private List<IAvatarTrackingOverride> _trackingOverrides;

        public AvatarArm(Handedness handedness, IBone dataRig, IBone physRig, IArmGroup dataArm, IArmGroup physArm)
        {
            Handedness = handedness;
            DataRig = dataRig;
            PhysRig = physRig;
            DataArm = dataArm;
            PhysArm = physArm;

            _trackingOverrides = new List<IAvatarTrackingOverride>();
            DataArm.OnProcessTarget += OnProcessTarget;
        }

        public readonly void RegisterTrackingOverride(IAvatarTrackingOverride trackingOverride)
        {
            _trackingOverrides.Add(trackingOverride);
        }

        public readonly void UnregisterTrackingOverride(IAvatarTrackingOverride trackingOverride)
        {
            _trackingOverrides.Remove(trackingOverride);
        }

        private readonly SimpleTransform OnProcessTarget(in SimpleTransform target)
        {
            SimpleTransform targetInRig = DataRig.Transform.InverseTransform(target);
            SimpleTransform physRig = PhysRig.Transform;

            foreach (var trackingOverride in _trackingOverrides)
            {
                targetInRig = trackingOverride.Solve(physRig, targetInRig);
            }

            return DataRig.Transform.Transform(targetInRig);
        }
    }
}
