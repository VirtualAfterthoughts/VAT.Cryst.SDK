using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IGrippable : IHostable
    {
        /// <summary>
        /// Invoked by this Grippable when it needs to detach a grabber.
        /// <para>Whenever a Grabber attaches to this Grippable, it should hook into this event to be notified.</para>
        /// </summary>
        event Action<IGrabber, IGrippable> OnForceDetachEvent;

        /// <summary>
        /// Invoked when this Grippable is attached to a Grabber.
        /// </summary>
        event Action<IGrabber> OnAttachedEvent;

        /// <summary>
        /// Invoked when this Grippable is detached from a Grabber.
        /// </summary>
        event Action<IGrabber> OnDetachedEvent;

        /// <summary>
        /// <para>
        /// This value is typically calculated by getting the transform of the grip target relative to the grip host.
        /// For example, it would evaluate to "host.InverseTransform(target)".
        /// The grip's CreateJoint function will automatically use this when necessary.
        /// </para>
        /// </summary>
        /// <param name="grabber"></param>
        /// <returns>The target transform of the grabber relative to the grip host.</returns>
        SimpleTransform GetGrabberInHost(IGrabber grabber);

        /// <summary>
        /// <para>
        /// This value is typically calculated by getting the transform of the grip host relative to the grip target.
        /// For example, it would evaluate to "target.InverseTransform(host)".
        /// The grip's CreateJoint function will automatically use this when necessary.
        /// </para>
        /// </summary>
        /// <param name="grabber"></param>
        /// <returns>The target transform of the grip host relative to the grabber.</returns>
        SimpleTransform GetHostInGrabber(IGrabber grabber);

        /// <summary>
        /// This value is the world space version of the Grippable's grab target.
        /// This is typically used by default in the GetGrabberInHost and GetHostInGrabber functions.
        /// </summary>
        /// <param name="grabber"></param>
        /// <returns>The target transform of the grab in world space.</returns>
        SimpleTransform GetTargetInWorld(IGrabber grabber);

        /// <summary>
        /// Returns if this Grippable can be attached to a Grabber.
        /// </summary>
        /// <param name="interactor"></param>
        /// <returns></returns>
        public bool CanAttach(IGrabber grabber);

        /// <summary>
        /// Attempts to detach a grabber.
        /// </summary>
        /// <param name="grabber">The grabber to detach.</param>
        void DetachGrabber(IGrabber grabber);
        
        /// <summary>
        /// Attempts to detach all attached grabbers.
        /// </summary>
        void DetachGrabbers();

        /// <summary>
        /// Invoked by a Grabber upon attaching this Grippable.
        /// </summary>
        /// <param name="grabber"></param>
        void OnAttached(IGrabber grabber);

        /// <summary>
        /// Invoked by a Grabber upon detaching this Grippable.
        /// </summary>
        /// <param name="grabber"></param>
        void OnDetached(IGrabber grabber);

        /// <summary>
        /// Creates a grab joint on the target grabber.
        /// </summary>
        /// <param name="grabber"></param>
        void CreateJoint(IGrabber grabber);

        /// <summary>
        /// Updates the joint settings of the grab joint.
        /// Should be called every frame by the grabber if using a joint.
        /// </summary>
        /// <param name="grabber"></param>
        void UpdateJoint(IGrabber grabber);

        /// <summary>
        /// Destroys the grab joint on the target grabber.
        /// </summary>
        /// <param name="grabber"></param>
        void DestroyJoint(IGrabber grabber);
    }
}
