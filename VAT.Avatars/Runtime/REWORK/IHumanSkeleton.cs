using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.REWORK
{
    public interface IHumanSkeleton : ISkeleton
    {
        public IHumanNeck Neck { get; }
        public IHumanSpine Spine { get; }
        public IHumanArm LeftArm { get; }
        public IHumanArm RightArm { get; }
        public IHumanLeg LeftLeg { get; }
        public IHumanLeg RightLeg { get; }
        public IBoneGroup LocoLeg { get; }
    }
}
