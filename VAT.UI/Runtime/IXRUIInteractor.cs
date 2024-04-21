using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.UI
{
    public interface IXRUIInteractor
    {
        bool IsPressed();

        Vector3 GetEndPosition();
    }
}
