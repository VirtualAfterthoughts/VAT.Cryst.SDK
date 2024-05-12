using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public class HostLink
    {
        public InteractableHost host;
        public InteractableHostGroup linkedGroup;

        public void Attach()
        {
            host.Link(this);

            foreach (var host in linkedGroup.hosts)
            {
                host.Link(this);
            }

            ToggleCollision(true);
        }

        public void Detach()
        {
            host.Unlink(this);

            foreach (var host in linkedGroup.hosts)
            {
                host.Unlink(this);
            }

            ToggleCollision(false);
        }

        public void ToggleCollision(bool ignore)
        {
            host.SelfGroup.IgnoreCollision(linkedGroup, ignore);

            foreach (var link in host.Links)
            {
                link.host.SelfGroup.IgnoreCollision(linkedGroup, ignore);

                link.linkedGroup.IgnoreCollision(linkedGroup, ignore);
            }
        }
    }
}
