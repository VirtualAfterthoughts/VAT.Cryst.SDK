using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

namespace VAT.Props
{
    public class SurfaceProperties : MonoBehaviour
    {
        [SerializeField]
        private ShardReferenceT<SurfaceMaterialShard> _materialReference;
    }
}
