using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Interaction.Entities;

namespace VAT.Interaction
{
    public interface IEntityChild
    {
        IEntity ParentEntity { get; set; }
    }
}
