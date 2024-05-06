using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Entities
{
    public interface IFreezable
    {
        void Freeze();

        void Unfreeze();
    }
}
