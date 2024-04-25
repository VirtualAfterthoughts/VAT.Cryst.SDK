using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Cryst.Utilities
{
    public static class MeshSimplifier
    {
        public static Mesh Simplify(GameObject root)
        {
            List<CombineInstance> combineInstances = new();

            var rootMatrix = root.transform.worldToLocalMatrix;

            var filters = root.GetComponentsInChildren<MeshFilter>();
            foreach (var filter in filters)
            {
                if (filter.sharedMesh == null)
                {
                    continue;
                }

                combineInstances.Add(new CombineInstance()
                {
                    mesh = filter.sharedMesh,
                    transform = rootMatrix.inverse * filter.transform.localToWorldMatrix,
                });
            }

            var combined = new Mesh();
            combined.CombineMeshes(combineInstances.ToArray(), true, true, false);

            var simplifier = new UnityMeshSimplifier.MeshSimplifier();
            simplifier.Initialize(combined);

            if (combined.vertexCount > 1000)
            {
                simplifier.SimplifyMesh(0.1f);
            }

            var simplified = simplifier.ToMesh();

            return simplified;
        }
    }
}
