using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VAT.Avatars.Art;
using VAT.Shared.Data;

namespace VAT.Characters
{
    public class PlayerRenderer : MonoBehaviour
    {
        public AvatarRig avatarRig;

        private void OnEnable()
        {
            avatarRig.OnPostArt += OnPostArt;
        }

        private void OnDisable()
        {
            avatarRig.OnPostArt -= OnPostArt;
        }

        private void OnPostArt()
        {
            var avatar = avatarRig.CurrentAvatar;
            if (avatar != null)
            {
                var skeleton = avatar.GetSkeleton().GetArt();
                var head = (ArtBone)skeleton.GetHead();
                var eyeCenter = skeleton.GetEyeCenter();

                var headTransform = head.Transform;

                float distance = math.length(headTransform.Position - eyeCenter.Position);

                headTransform.Position -= headTransform.Forward * distance * 0.5f;

                head.Transform = headTransform;
            }
        }
    }
}
