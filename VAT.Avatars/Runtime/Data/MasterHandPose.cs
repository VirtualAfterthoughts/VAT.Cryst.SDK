using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Data
{
    [CreateAssetMenu(fileName = "New Master Hand Pose", menuName = "VAT/Data/Master Hand Pose")]
    public sealed class MasterHandPose : ScriptableObject {
        private readonly List<IHandPose> _poses = new();
        private readonly Dictionary<string, IHandPose> _poseLookup = new();

        public IReadOnlyList<IHandPose> Poses => _poses;

        public void InjectPose(IHandPose pose) {
            if (_poseLookup.ContainsKey(pose.Address)) {
                Debug.LogWarning($"Master Hand Pose {name} contains multiple poses at address {pose.Address}!", this);
                return;
            }

            _poses.Add(pose);
            _poseLookup.Add(pose.Address, pose);
        }

        public bool TryGetPose(string address, out IHandPose pose) { 
            return _poseLookup.TryGetValue(address, out pose);
        }

        public bool TryGetPose<TPose>(string address, out TPose pose) where TPose : IHandPose {
            pose = default;

            if (_poseLookup.TryGetValue(address, out var result) && result is TPose generic) {
                pose = generic;
                return true;
            }

            return false;
        }
    }
}
