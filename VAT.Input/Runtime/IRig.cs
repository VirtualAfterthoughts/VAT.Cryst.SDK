using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Input
{
    public interface IRigT<TBone> {
        // Head
        /// <summary>
        /// Attempts to get a head bone.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetHead(out TBone result);

        /// <summary>
        /// Gets an array of all head bones.
        /// </summary>
        /// <returns></returns>
        public TBone[] GetHeads();

        // Hands
        /// <summary>
        /// Attempts to get a hand bone with a specific handedness.
        /// </summary>
        /// <param name="handedness"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetHand(Handedness handedness, out TBone result);

        /// <summary>
        /// Gets an array of all hand bones with a specific handedness.
        /// </summary>
        /// <param name="handedness"></param>
        /// <returns></returns>
        public TBone[] GetHands(Handedness handedness);

        // Pelvis
        /// <summary>
        /// Attempts to get a pelvis bone.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetPelvis(out TBone result);

        /// <summary>
        /// Gets an array of all pelvis bones.
        /// </summary>
        /// <returns></returns>
        public TBone[] GetPelvises();

        // Feet
        /// <summary>
        /// Attempts to get a foot bone with a specific handedness.
        /// </summary>
        /// <param name="handedness"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetFoot(Handedness handedness, out TBone result);

        /// <summary>
        /// Gets an array of all foot bones with a specific handedness.
        /// </summary>
        /// <param name="handedness"></param>
        /// <returns></returns>
        public TBone[] GetFeet(Handedness handedness);

        // Root
        // Every rig requires a root.
        /// <summary>
        /// Gets the root bone.
        /// </summary>
        /// <returns></returns>
        public TBone GetRoot();
    }

    public interface IRigTransform : IRigT<SimpleTransform> {

    }
}
