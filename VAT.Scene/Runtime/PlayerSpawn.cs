using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Cryst.Game;

namespace VAT.Scene
{
    public class PlayerSpawn : MonoBehaviour
    {
        public void Awake()
        {
            var settings = CrystSettings.LoadedSettings;

            if (settings != null)
            {
                Debug.Log("YAY THEY EXIST!");
            }
            else
            {
                Debug.Log("Oh..");
            }
        }
    }
}
