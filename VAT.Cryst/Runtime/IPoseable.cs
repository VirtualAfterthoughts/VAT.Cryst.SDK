using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Cryst
{
    public interface IPoseableT<TReference>
    {
        void MatchPose(TReference reference);
    }
}
