using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;

namespace VAT.Avatars.Integumentary
{
    public abstract partial class Avatar : MonoBehaviour
    {
        private Dictionary<Handedness, List<AvatarArm>> _armLookup = null;

        private Dictionary<Handedness, List<AvatarLeg>> _legLookup = null;

        public void InitiateLimbs() {
            // Arms
            _armLookup = new();

            foreach (var hand in CreateArms()) {
                if (!_armLookup.ContainsKey(hand.Handedness))
                    _armLookup.Add(hand.Handedness, new List<AvatarArm>());

                _armLookup[hand.Handedness].Add(hand);
            }

            // Legs
            _legLookup = new();

            foreach (var leg in CreateLegs())
            {
                if (!_legLookup.ContainsKey(leg.Handedness))
                {
                    _legLookup.Add(leg.Handedness, new List<AvatarLeg>());
                }

                _legLookup[leg.Handedness].Add(leg);
            }
        }

        public void UninitiateLimbs() {
            // Arms
            _armLookup = null;

            // Legs
            _legLookup = null;
        }

        protected virtual AvatarArm[] CreateArms() {
            return Array.Empty<AvatarArm>();
        }

        protected virtual AvatarLeg[] CreateLegs()
        {
            return Array.Empty<AvatarLeg>();
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
