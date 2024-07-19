using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class TagReference : ShardReferenceT<Tag>
    {
    }

    public class Tag : DataShard
    {
    }

    [Serializable]
    public class TagMask
    {
        [SerializeField]
        private List<TagReference> _tags = new();

        public List<TagReference> Tags => _tags;

        public bool HasTag(TagReference tag)
        {
            return Tags.Any((other) => other.Address == tag.Address);
        }
    }
}
