using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VAT.Packaging;

namespace VAT.Interaction
{
    public class SpawnUI : MonoBehaviour
    {
        public static SpawnableContentReference SelectedSpawnable = null;

        public Button[] buttons;

        private void Awake()
        {
            AssetPackager.HookOnReady(OnPackagerReady);
        }

        private void OnPackagerReady()
        {
            foreach (var button in buttons)
            {
                button.gameObject.SetActive(false);
            }

            var contents = AssetPackager.Instance.GetContents<ISpawnableContent>();
            for (var i = 0; i < buttons.Length && i < contents.Count; i++)
            {
                buttons[i].gameObject.SetActive(true);

                var content = contents.ElementAt(i);
                var address = content.Address;

                buttons[i].onClick.AddListener(() =>
                {
                    SelectSpawnable(address);
                });

                var tmp = buttons[i].GetComponentInChildren<TMP_Text>();

                if (tmp != null)
                {
                    tmp.text = content.Info.Title;
                }
            }
        }

        private void SelectSpawnable(Address address)
        {
            SelectedSpawnable = new SpawnableContentReference(address);
        } 
    }
}
