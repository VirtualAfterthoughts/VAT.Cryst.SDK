using UnityEngine;

namespace VAT.Shared.Extensions
{
    /// <summary>
    /// Extension methods for GameObjects.
    /// </summary>
    public static partial class GameObjectExtensions
    {
        /// <summary>
        /// Creates a new GameObject as a child of a transform.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreateGameObject(string name, Transform parent)
        {
            GameObject go = new(name);
            Transform tran = go.transform;
            tran.parent = parent;
            tran.Reset();
            return go;
        }

        /// <summary>
        /// Attempts to get the component in parents.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="_this"></param>
        /// <param name="component">The component result.</param>
        /// <returns>The success.</returns>
        public static bool TryGetComponentInParent<T>(this GameObject _this, out T component) where T : Component
            => (component = _this.GetComponentInParent<T>()) != null;

        /// <summary>
        /// If a component is found, it is returned. Otherwise, a new component is added.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="go">The GameObject.</param>
        /// <returns>The found or added component.</returns>
        public static T AddOrGetComponent<T>(this GameObject go) where T : Component
        {
            if (!go.TryGetComponent(out T comp))
                comp = go.AddComponent(typeof(T)) as T;
            return comp;
        }
    }
}
