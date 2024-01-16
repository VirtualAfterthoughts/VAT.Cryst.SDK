using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;

namespace VAT.Avatars.Integumentary
{
    public abstract partial class Avatar : MonoBehaviour, IAvatar
    {
        private Dictionary<Handedness, List<IAvatarHand>> _handLookup = null;

        public void InitiateLimbs() {
            // Hands
            _handLookup = new();

            foreach (var hand in CreateHands()) {
                if (!_handLookup.ContainsKey(hand.Handedness))
                    _handLookup.Add(hand.Handedness, new List<IAvatarHand>());

                _handLookup[hand.Handedness].Add(hand);
            }
        }

        public void UninitiateLimbs() {
            // Hands
            _handLookup = null;
        }

        protected virtual IAvatarHand[] CreateHands() {
            return Array.Empty<IAvatarHand>();
        }

        public bool TryGetHand(Handedness handedness, out IAvatarHand hand) {
            hand = null;
            
            if (_handLookup.TryGetValue(handedness, out var list)) {
                hand = list[0];
                return true;
            }

            return false;
        }

        public IAvatarHand[] GetHands(Handedness handedness) {
            if (_handLookup.TryGetValue(handedness, out var list))
                return list.ToArray();
            return Array.Empty<IAvatarHand>();
        }

        public IAvatarHand[] GetHands() {
            List<IAvatarHand> hands = null;

            foreach (var list in _handLookup.Values) {
                hands ??= new();
                hands.AddRange(list);
            }

            return hands.ToArray();
        }
    }
}
