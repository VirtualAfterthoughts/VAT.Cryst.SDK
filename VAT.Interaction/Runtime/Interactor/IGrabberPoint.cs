using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IGrabberPoint
    {
        SimpleTransform GetDefaultGrabPoint();

        SimpleTransform GetGrabPoint(Vector2 position);

        Vector3 GetGrabNormal();

        SimpleTransform GetGrabCenter();

        SimpleTransform GetParentTransform();
    }
}
