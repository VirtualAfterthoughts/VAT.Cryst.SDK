using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;

namespace VAT.Avatars.Integumentary
{
    public abstract partial class Avatar : MonoBehaviour, IAvatar
    {
        private Dictionary<Handedness, List<AvatarArm>> _armLookup = null;

        public void InitiateLimbs() {
            // Arms
            _armLookup = new();

            foreach (var hand in CreateArms()) {
                if (!_armLookup.ContainsKey(hand.Handedness))
                    _armLookup.Add(hand.Handedness, new List<AvatarArm>());

                _armLookup[hand.Handedness].Add(hand);
            }
        }

        public void UninitiateLimbs() {
            // Arms
            _armLookup = null;
        }

        protected virtual AvatarArm[] CreateArms() {
            return Array.Empty<AvatarArm>();
        }

        public bool TryGetArm(Handedness handedness, out AvatarArm arm) {
            arm = default;
            
            if (_armLookup.TryGetValue(handedness, out var list)) {
                arm = list[0];
                return true;
            }

            return false;
        }

        public AvatarArm[] GetArms(Handedness handedness) {
            if (_armLookup.TryGetValue(handedness, out var list))
                return list.ToArray();
            return Array.Empty<AvatarArm>();
        }

        public AvatarArm[] GetArms() {
            List<AvatarArm> arms = null;

            foreach (var list in _armLookup.Values) {
                arms ??= new();
                arms.AddRange(list);
            }

            return arms.ToArray();
        }
    }
}
