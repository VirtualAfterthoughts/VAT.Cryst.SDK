using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Data {
    public interface IHandPose {
        /// <summary>
        /// The unique identifier for this type of hand pose.
        /// </summary>
        public string Address { get; }
    }
}
