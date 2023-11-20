using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Shared.Math {
    public struct MeshTriangle {
        public int v1;
        public int v2;
        public int v3;

        public MeshTriangle(int v1, int v2, int v3) {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public static MeshTriangle Offset(MeshTriangle triangle, int offset) {
            return new MeshTriangle(triangle.v1 + offset, triangle.v2 + offset, triangle.v3 + offset);
        }
    }

    public static partial class MeshHelper {
        public static void SetTriangles(this Mesh mesh, IList<MeshTriangle> triangles, int submesh = 0) {
            var indicies = new List<int>();
            for (int i = 0; i < triangles.Count; i++) {
                indicies.Add(triangles[i].v1);
                indicies.Add(triangles[i].v2);
                indicies.Add(triangles[i].v3);
            }
            mesh.SetTriangles(indicies, submesh);
        }
    }
}
