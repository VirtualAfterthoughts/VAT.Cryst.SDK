using System;

using Unity.Mathematics;
using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Shared.Extensions {
#if UNITY_EDITOR
    public class TempGizmoColor : IDisposable {
        public Color color;

        private TempGizmoColor() { }

        public static TempGizmoColor Create() {
            return new TempGizmoColor()
            {
                color = Gizmos.color
            };
        }

        public void Dispose() {
            Gizmos.color = color;
        }
    }

    public class TempGizmoMatrix : IDisposable
    {
        public Matrix4x4 matrix;

        private TempGizmoMatrix() { }

        public static TempGizmoMatrix Create()
        {
            return new TempGizmoMatrix()
            {
                matrix = Gizmos.matrix
            };
        }

        public void Dispose()
        {
            Gizmos.matrix = matrix;
        }
    }

    /// <summary>
    /// Extension methods for drawing gizmos.
    /// </summary>
    public static class GizmosExtensions {
        /// <summary>
        /// Draws the Gizmos of every mesh on this GameObject.
        /// </summary>
        /// <param name="go">The GameObject to draw.</param>
        /// <param name="transform">The target position and rotations.</param>
        /// <param name="isWireframe">Should this draw as a wireframe?</param>
        public static void DrawGameObject(this GameObject go, Transform transform, bool isWireframe = true)
            => go.DrawGameObject(SimpleTransform.Create(transform), isWireframe);

        /// <summary>
        /// Draws the Gizmos of every mesh on this GameObject.
        /// </summary>
        /// <param name="go">The GameObject to draw.</param>
        /// <param name="transform">The target position and rotations.</param>
        /// <param name="color">The color to draw.</param>
        /// <param name="isWireframe">Should this draw as a wireframe?</param>
        public static void DrawGameObject(this GameObject go, Transform transform, Color color, bool isWireframe = true) 
            => go.DrawGameObject(SimpleTransform.Create(transform), color, isWireframe);

        /// <summary>
        /// Draws the Gizmos of every mesh on this GameObject.
        /// </summary>
        /// <param name="go">The GameObject to draw.</param>
        /// <param name="transform">The target position and rotations.</param>
        /// <param name="isWireframe">Should this draw as a wireframe?</param>
        public static void DrawGameObject(this GameObject go, SimpleTransform transform, bool isWireframe = true) {
            // Draw all of the object's meshes
            InternalDrawMeshRenderers(go, transform, isWireframe);
            InternalDrawSkinnedMeshes(go, transform, isWireframe);
        }

        /// <summary>
        /// Draws the Gizmos of every mesh on this GameObject.
        /// </summary>
        /// <param name="go">The GameObject to draw.</param>
        /// <param name="transform">The target position and rotations.</param>
        /// <param name="color">The color to draw.</param>
        /// <param name="isWireframe">Should this draw as a wireframe?</param>
        public static void DrawGameObject(this GameObject go, SimpleTransform transform, Color color, bool isWireframe = true) {
            // Make the gizmo color reset when this goes out of scope
            using (TempGizmoColor.Create())
            {
                // Set the color
                Gizmos.color = color;

                // Draw the meshes
                DrawGameObject(go, transform, isWireframe);
            }
        }

        private static void InternalDrawMeshRenderers(GameObject go, SimpleTransform transform, bool isWireframe = true) {
            if (Application.isPlaying)
                return;

            MeshFilter[] meshes = go.GetComponentsInChildren<MeshFilter>();
            Matrix4x4 newMatrix = transform.localToWorldMatrix;
            Matrix4x4 original = Gizmos.matrix;

            // Loop through all mesh filters and draw their meshes
            for (int i = 0; i < meshes.Length; i++) {
                // Make sure this filter has an assigned mesh
                MeshFilter mesh = meshes[i];
                if (!mesh.sharedMesh) 
                    continue;

                // Get offset matrix
                Transform root = mesh.transform.root;
                Vector3 position = mesh.transform.position;
                Quaternion rotation = mesh.transform.rotation;
                Vector3 scale = mesh.transform.lossyScale;

                position -= root.position;
                rotation = Quaternion.Inverse(root.rotation) * rotation;
                scale = (float3)scale / (float3)root.lossyScale;

                Matrix4x4 meshMatrix = Matrix4x4.TRS(position, rotation, scale);

                // Draw the mesh
                Gizmos.matrix = newMatrix * meshMatrix;

                if (isWireframe)
                    Gizmos.DrawWireMesh(mesh.sharedMesh);
                else
                    Gizmos.DrawMesh(mesh.sharedMesh);
            }

            Gizmos.matrix = original;
        }

        private static void InternalDrawSkinnedMeshes(GameObject go, SimpleTransform transform, bool isWireframe = true) {
            if (Application.isPlaying)
                return;

            SkinnedMeshRenderer[] meshes = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            Matrix4x4 newMatrix = transform.localToWorldMatrix;
            Matrix4x4 original = Gizmos.matrix;

            // Loop through all of the skinned mesh renderers
            for (int i = 0; i < meshes.Length; i++) {
                // Make sure the skinned mesh renderer has an assigned mesh
                SkinnedMeshRenderer mesh = meshes[i];
                if (!mesh.sharedMesh) 
                    continue;

                // Get offset matrix
                Transform root = mesh.transform.root;
                Vector3 position = mesh.transform.position;
                Quaternion rotation = mesh.transform.rotation;
                Vector3 scale = mesh.transform.lossyScale;

                position -= root.position;
                rotation = Quaternion.Inverse(root.rotation) * rotation;
                scale = (float3)scale / (float3)root.lossyScale;

                Matrix4x4 meshMatrix = Matrix4x4.TRS(position, rotation, scale);

                // Draw the mesh
                Gizmos.matrix = newMatrix * meshMatrix;
                if (isWireframe)
                    Gizmos.DrawWireMesh(mesh.sharedMesh);
                else
                    Gizmos.DrawMesh(mesh.sharedMesh);
            }

            Gizmos.matrix = original;
        }
    }
#endif
}
