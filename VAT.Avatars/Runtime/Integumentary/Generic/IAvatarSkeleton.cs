using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;
using VAT.Avatars.Art;

namespace VAT.Avatars.Integumentary
{
    public interface IAvatarSkeleton {
        DataBoneSkeleton GetData();

        PhysBoneSkeleton GetPhysics();

        ArtBoneSkeleton GetArt();
    }
}
