using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Entities
{
    public interface IEntityComponent
    {
        ICrystEntity Entity { get; }

        bool HasEntity { get; }
    }
}
