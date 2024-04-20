using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Cryst.State;

namespace VAT.Input
{
    public class HandActions
    {
        public BoolState GrabAction = new();

        public BoolState AbilityGrabAction = new();

        public BoolState PrimaryAction = new();

        public BoolState SecondaryAction = new();
    }
}
