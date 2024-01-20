using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;
using VAT.Shared.Data;

namespace VAT.Avatars.Nervous {
    public interface IAvatarPayload {
        public bool TryGetHead(out SimpleTransform result);

        public SimpleTransform[] GetHeads();

        public bool TryGetArm(Handedness handedness, out IArm result);

        public IArm[] GetArms(Handedness handedness);

        public bool TryGetPelvis(out SimpleTransform result);

        public SimpleTransform[] GetPelvises();

        public bool TryGetFoot(Handedness handedness, out SimpleTransform result);

        public SimpleTransform[] GetFeet(Handedness handedness);

        public SimpleTransform GetRoot();

        void SetRoot(SimpleTransform root);

        bool TryGetInput(out IAvatarInput input);
    }

    public struct BasicAvatarPayload : IAvatarPayload
    {
        public SimpleTransform Root { get; set; }

        public SimpleTransform Head { get; set; }

        public IArm LeftArm { get; set; }

        public IArm RightArm { get; set; }

        public IAvatarInput Input { get; set; }

        public readonly SimpleTransform[] GetFeet(Handedness handedness)
        {
            return Array.Empty<SimpleTransform>();
        }

        public readonly IArm[] GetArms(Handedness handedness)
        {
            return handedness switch
            {
                Handedness.LEFT => new IArm[] { LeftArm },
                Handedness.RIGHT => new IArm[] { RightArm },
                Handedness.BOTH => new IArm[] { LeftArm, RightArm },
                _ => Array.Empty<IArm>(),
            };;
        }

        public readonly SimpleTransform[] GetHeads()
        {
            return new SimpleTransform[] { Head };
        }

        public readonly SimpleTransform[] GetPelvises()
        {
            return Array.Empty<SimpleTransform>();
        }

        public readonly SimpleTransform GetRoot()
        {
            return Root;
        }

        public void SetRoot(SimpleTransform value)
        {
            Root = value;
        }

        public readonly bool TryGetFoot(Handedness handedness, out SimpleTransform result)
        {
            result = SimpleTransform.Default;
            return false;
        }

        public readonly bool TryGetArm(Handedness handedness, out IArm result)
        {
            switch (handedness)
            {
                default:
                    result = default;
                    return false;
                case Handedness.LEFT:
                    result = LeftArm;
                    return true;
                case Handedness.RIGHT:
                    result = RightArm;
                    return true;
            }
        }

        public readonly bool TryGetHead(out SimpleTransform result)
        {
            result = Head;
            return true;
        }

        public readonly bool TryGetPelvis(out SimpleTransform result)
        {
            result = SimpleTransform.Default;
            return false;
        }

        public readonly bool TryGetInput(out IAvatarInput input)
        {
            input = Input;
            return input != null;
        }
    }
}
