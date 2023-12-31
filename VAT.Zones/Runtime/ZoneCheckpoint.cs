using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using VAT.Cryst;
using VAT.Entities;

namespace VAT.Zones
{
    public sealed class ZoneCheckpoint : ZoneComponent, ICheckpoint
    {
        [SerializeField]
        private UnityEvent<EntityTracker> _onCheckpointActivate;

        public SimpleCheckpoint CheckpointTransform => new(transform.position, transform.rotation);

        private bool _hasBeenUsed = false;

        public override void OnPrimaryZoneEntered(EntityTracker tracker)
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
