using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace VAT.Interaction
{
    public class InteractableHostGroup
    {
        public List<InteractableHost> hosts;

        public InteractableHostGroup(params InteractableHost[] hosts)
        {
            this.hosts = hosts.ToList();
        }

        public InteractableHostGroup(List<InteractableHost> hosts)
        {
            this.hosts = hosts;
        }

        public void IgnoreCollision(InteractableHostGroup other, bool ignore)
        {
            foreach (var selfHost in hosts)
            {
                var selfColliders = selfHost.GetColliders();

                foreach (var otherHost in other.hosts)
                {
                    var otherColliders = otherHost.GetColliders();

                    foreach (var selfCollider in selfColliders)
                    {
                        foreach (var otherCollider in otherColliders)
                        {
                            Physics.IgnoreCollision(selfCollider, otherCollider, ignore);
                        }
                    }
                }
            }
        }
    }
}
