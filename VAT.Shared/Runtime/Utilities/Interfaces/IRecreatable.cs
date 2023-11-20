using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Shared.Utilities
{
    public interface IRecreatable {
        void CreateItem();

        void DestroyItem();
    }
}
