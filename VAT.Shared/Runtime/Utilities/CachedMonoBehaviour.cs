using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Shared {
    /// <summary>
    /// An interface to implement for caching the behaviour's GameObject and Transform variables.
    /// </summary>
    public interface ICachedMonoBehaviour {
        GameObject GameObject { get; }
        Transform Transform { get; }
    }

    /// <summary>
    /// An implementation of <see cref="ICachedMonoBehaviour"/>.
    /// </summary>
    public class CachedMonoBehaviour : MonoBehaviour, ICachedMonoBehaviour {
        private GameObject _gameObject = null;
        private bool _hasGameObject = false;
        public GameObject GameObject {
            get  {
                if (!_hasGameObject)
                {
                    _gameObject = base.gameObject;
                    _hasGameObject = true;
                }

                return _gameObject;
            } 
        }

        private Transform _transform = null;
        private bool _hasTransform = false;
        public Transform Transform {
            get {
                if (!_hasTransform)
                {
                    _transform = base.transform;
                    _hasTransform = true;
                }

                return _transform;
            }
        }
    }
}
