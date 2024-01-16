using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;
using VAT.Shared.Data;

namespace VAT.Avatars.Nervous {
    public interface IAvatarPayload : IRigTransform {
        void SetRoot(SimpleTransform root);

        bool TryGetInput(out IBodyInput input);
    }

    public struct BasicAvatarPayload : IAvatarPayload
    {
        public SimpleTransform Root { get; set; }

        public SimpleTransform Head { get; set; }

        public SimpleTransform LeftHand { get; set; }

        public SimpleTransform RightHand { get; set; }

        public IBodyInput Input { get; set; }

        readonly SimpleTransform[] IRigT<SimpleTransform>.GetFeet(Handedness handedness)
        {
            return Array.Empty<SimpleTransform>();
        }

        readonly SimpleTransform[] IRigT<SimpleTransform>.GetHands(Handedness handedness)
        {
            return handedness switch
            {
                Handedness.LEFT => new SimpleTransform[] { LeftHand },
                Handedness.RIGHT => new SimpleTransform[] { RightHand },
                _ => Array.Empty<SimpleTransform>(),
            };
        }

        readonly SimpleTransform[] IRigT<SimpleTransform>.GetHeads()
        {
            return new SimpleTransform[] { Head };
        }

        readonly SimpleTransform[] IRigT<SimpleTransform>.GetPelvises()
        {
            return Array.Empty<SimpleTransform>();
        }

        readonly SimpleTransform IRigT<SimpleTransform>.GetRoot()
        {
            return Root;
        }

        public void SetRoot(SimpleTransform value)
        {
            Root = value;
        }

        readonly bool IRigT<SimpleTransform>.TryGetFoot(Handedness handedness, out SimpleTransform result)
        {
            result = SimpleTransform.Default;
            return false;
        }

        readonly bool IRigT<SimpleTransform>.TryGetHand(Handedness handedness, out SimpleTransform result)
        {
            switch (handedness)
            {
                default:
                    result = SimpleTransform.Default;
                    return false;
                case Handedness.LEFT:
                    result = LeftHand;
                    return true;
                case Handedness.RIGHT:
                    result = RightHand;
                    return true;
            }
        }

        readonly bool IRigT<SimpleTransform>.TryGetHead(out SimpleTransform result)
        {
            result = Head;
            return true;
        }

        readonly bool IRigT<SimpleTransform>.TryGetPelvis(out SimpleTransform result)
        {
            result = SimpleTransform.Default;
            return false;
        }

        public readonly bool TryGetInput(out IBodyInput input)
        {
            input = Input;
            return input != null;
        }
    }
}
