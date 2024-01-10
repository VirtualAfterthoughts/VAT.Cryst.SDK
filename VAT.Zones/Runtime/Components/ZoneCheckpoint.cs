using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using VAT.Cryst;
using VAT.Entities;
using VAT.Shared.Data;

namespace VAT.Zones
{
    public sealed class ZoneCheckpoint : ZoneComponent, ICheckpoint
    {
        [SerializeField]
        private UnityEvent<CrystEntityTracker> _onCheckpointActivate;

        public SimpleTransform CheckpointTransform => SimpleTransform.Create(transform.position, transform.rotation);

        private bool _hasBeenUsed = false;

        public override void OnPrimaryZoneEntered(CrystEntityTracker tracker)
        {
            if (_hasBeenUsed)
                return;

            if (ICheckpointable.Cache.TryGet(tracker.Entity.GetEntityGameObject(), out var checkpointable))
            {
                checkpointable.SetCheckpoint(this);
                _onCheckpointActivate.Invoke(tracker);

                _hasBeenUsed = true;
            }
        }
    }
}
