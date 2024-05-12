using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IPalm
    {
        SimpleTransform GetDefaultPoint();

        SimpleTransform GetPoint(Vector2 position);

        Vector3 GetNormal();

        SimpleTransform GetProximityCenter();

        SimpleTransform GetHostTransform();
    }
}
