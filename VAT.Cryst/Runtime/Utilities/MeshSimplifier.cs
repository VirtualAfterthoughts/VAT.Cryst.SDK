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

            var rootMatrix = Matrix4x4.TRS(root.transform.position, root.transform.rotation, Vector3.one);

            var filters = root.GetComponentsInChildren<MeshFilter>();
            foreach (var filter in filters)
            {
                if (!filter.TryGetComponent<MeshRenderer>(out var renderer))
                {
                    continue;
                }

                if (!renderer.enabled)
                {
                    continue;
                }

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

            List<Mesh> tempBakeMeshes = new();

            var skins = root.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var skin in skins)
            {
                if (!skin.enabled)
                {
                    continue;
                }

                if (skin.sharedMesh == null)
                {
                    continue;
                }

                Mesh tempMesh = new();

                skin.BakeMesh(tempMesh, true);

                tempBakeMeshes.Add(tempMesh);

                var transform = rootMatrix.inverse * skin.transform.localToWorldMatrix;

                for (var i = 0; i < tempMesh.subMeshCount; i++)
                {
                    combineInstances.Add(new CombineInstance()
                    {
                        mesh = tempMesh,
                        subMeshIndex = i,
                        transform = transform,
                    });
                }
            }

            var combined = new Mesh();
            combined.CombineMeshes(combineInstances.ToArray(), true, true, false);

            var simplifier = new UnityMeshSimplifier.MeshSimplifier();
            simplifier.Initialize(combined);

            if (combined.vertexCount > 1000)
            {
                simplifier.SimplifyMesh(0.5f);
            }

            var simplified = simplifier.ToMesh();

            simplified.RecalculateNormals();

            foreach (var temp in tempBakeMeshes)
            {
                Object.DestroyImmediate(temp);
            }

            return simplified;
        }
    }
}
