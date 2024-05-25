using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public class HostLink
    {
        public enum LinkType
        {
            INTERNAL = 0,
            EXTERNAL = 1,
        }

        public InteractableHost host;
        public InteractableHostGroup linkedGroup;
        public LinkType linkType = LinkType.INTERNAL;

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
                if (link.linkType == linkType)
                {
                    continue;
                }

                link.host.SelfGroup.IgnoreCollision(linkedGroup, ignore);

                link.linkedGroup.IgnoreCollision(linkedGroup, ignore);

                foreach (var host in linkedGroup.hosts)
                {
                    foreach (var otherLink in host.Links)
                    {
                        if (otherLink.linkType == link.linkType)
                        {
                            continue;
                        }

                        link.host.SelfGroup.IgnoreCollision(otherLink.host.SelfGroup, ignore);
                        link.host.SelfGroup.IgnoreCollision(otherLink.linkedGroup, ignore);

                        link.linkedGroup.IgnoreCollision(otherLink.host.SelfGroup, ignore);
                        link.linkedGroup.IgnoreCollision(otherLink.linkedGroup, ignore);
                    }
                }
            }
        }
    }
}
