using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Nervous;
using VAT.Avatars.Posing;
using VAT.Input.Data;
using VAT.Shared.Extensions;

namespace VAT.Avatars.Integumentary {
    public abstract partial class Avatar : MonoBehaviour
    {
        public const string PhysSkeletonName = "[Rig - Physics]";

        public bool Initiated => _initiated;

        protected bool _initiated = false;

        public abstract IAvatarSkeleton GetSkeleton();

        public bool Uninitiate() {
            if (!_initiated)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Attempted to uninitiate an Avatar that wasn't initiated!", this);
#endif
                return false;
            }

            OnUninitiate();

#if UNITY_EDITOR
            if (Application.isPlaying) {
#endif
                OnUninitiateRuntime();
                UninitiateLimbs();
#if UNITY_EDITOR
            }
#endif

            _initiated = false;

#if UNITY_EDITOR
            if (Application.isPlaying)
                Debug.Log($"Successfully uninitiated Avatar {name}!", this);
#endif

            return true;
        }

        public bool Initiate()
        {
            if (_initiated)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Attempted to initiate an Avatar that is already initiated!", this);
#endif
                return false;
            }

            OnInitiate();

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                OnInitiateRuntime();
                InitiateLimbs();
#if UNITY_EDITOR
            }
#endif

            _initiated = true;

#if UNITY_EDITOR
            if (Application.isPlaying)
                Debug.Log($"Successfully initiated Avatar {name}!", this);
#endif

            return true;
        }

        public virtual void Write(IAvatarPayload payload)
        {
            GetSkeleton().GetData().Write(payload);
        }

        public void SolveArt() {
            GetSkeleton().GetArt().Solve();
        }

        /// <summary>
        /// Writes the position and rotation offsets between the DataBoneSkeleton and the ArtBoneSkeleton.
        /// </summary>
        public abstract void WriteArtOffsets();

        protected virtual void OnInitiate() { }

        protected virtual void OnInitiateRuntime() { }

        protected virtual void OnUninitiate() { }

        protected virtual void OnUninitiateRuntime() { }

        /// <summary>
        /// Tries to create a HandPoser out of this Avatar.
        /// </summary>
        /// <param name="poser">The resulting poser.</param>
        /// <returns>Whether or not a poser was successfully created.</returns>
        public virtual bool TryCreateHandPoser(out HandPoser poser) {
            poser = null;
            return false;
        }

        public abstract BodyMeasurements GetMeasurements();

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos() {
            if (Initiated) {
                using TempGizmoColor color = TempGizmoColor.Create();
                Gizmos.color = Color.green;

                GetSkeleton().GetData().DrawGizmos();
            }
        }

        protected virtual void OnValidate() {
            if (Application.isPlaying)
                return;

            if (_initiated)
                Uninitiate();

            Initiate();
        }

        public void EditorRefreshAvatar() {
            OnValidate();
        }
#endif
    }
}
