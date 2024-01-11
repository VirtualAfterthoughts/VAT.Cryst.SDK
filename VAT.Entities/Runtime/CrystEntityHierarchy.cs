using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Entities
{
    public class CrystEntityHierarchy
    {
        private readonly List<ICrystBody> _bodies = new();
        public IReadOnlyList<ICrystBody> Bodies => _bodies;

        private readonly List<ICrystJoint> _joints = new();
        public IReadOnlyList<ICrystJoint> Joints => _joints;

        public void AddBody(ICrystBody body)
        {
            _bodies.Add(body);
        }

        public bool RemoveBody(ICrystBody body) { 
            return _bodies.Remove(body);
        }

        public void AddJoint(ICrystJoint joint)
        {
            _joints.Add(joint);
        }

        public bool RemoveJoint(ICrystJoint joint)
        {
            return _joints.Remove(joint);
        }
    }
}
