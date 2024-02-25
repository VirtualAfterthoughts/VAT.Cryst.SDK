using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IVirtualControllerOverride
    {
        void OnSolveController(VirtualControllerPayload payload); 
    }
}
