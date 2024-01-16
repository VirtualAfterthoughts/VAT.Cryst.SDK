using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Input
{
    public interface IBodyInput : IBasicInput
    {
        public InputHand[] GetHands();

        public InputHand[] GetHands(Handedness handedness);
    }
}
