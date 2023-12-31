using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Cryst.Interfaces
{
    public interface IPoseableT<TReference>
    {
        void MatchPose(TReference reference);
    }
}
