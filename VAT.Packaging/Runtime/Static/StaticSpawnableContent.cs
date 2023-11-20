using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Extensions;

namespace VAT.Packaging
{
    [StaticContentIdentifier("Spawnable", typeof(GameObject))]
    public class StaticSpawnableContent : StaticGameObjectContent, ISpawnableContent
    {
#if UNITY_EDITOR
        public override string Group => "Spawnables";
#endif
    }
}
