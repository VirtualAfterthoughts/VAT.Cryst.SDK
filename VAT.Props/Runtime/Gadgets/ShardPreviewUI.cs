using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VAT.Packaging;

namespace VAT.Props
{
    public class ShardPreviewUI : MonoBehaviour
    {
        public TMP_Text crystalText;
        public TMP_Text shardText;
        public TMP_Text description;
        public RawImage shardIcon;

        public void SetShard(IShard shard)
        {
            if (crystalText != null)
            {
                crystalText.text = shard.MainCrystal?.Info.Title;
            }

            if (shardText != null)
            {
                shardText.text = shard.Info.Title;
            }

            if (description != null)
            {
                description.text = shard.Info.Description;
            }

            if (shardIcon != null && shard is IGameObjectShard go)
            {
                go.PreviewIcon?.LoadAsset((i) =>
                {
                    shardIcon.texture = i;
                });
            }
        }
    }
}
