using Cysharp.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.Management;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace VAT.Input.XR
{
    public static class XRManager
    {
        public static XRApi Api { get; private set; }

        private static void OnApplicationQuit()
        {
            DeinitializeApi();
        }

        public static bool IsInitialized()
        {
            var manager = XRGeneralSettings.Instance.Manager;

            return manager.activeLoader != null;
        }

        public static bool HasApi()
        {
            return Api != null;
        }

        public static async void InitializeApiAsync()
        {
            if (IsInitialized())
            {
                return;
            }

            var manager = XRGeneralSettings.Instance.Manager;

            Debug.Log("Initializing XRApi");

            var timer = Stopwatch.StartNew();

            await manager.InitializeLoader().ToUniTask();
            manager.StartSubsystems();

            Api = new XRApi();

            timer.Stop();

            Debug.Log($"Finished initializing, Elapsed {timer.Elapsed.TotalSeconds} seconds");

            Application.quitting += OnApplicationQuit;
        }
        
        public static void DeinitializeApi()
        {
            Application.quitting -= OnApplicationQuit;

            if (!IsInitialized())
            {
                return;
            }

            var manager = XRGeneralSettings.Instance.Manager;

            Debug.Log("Deinitializing XRApi");

            var timer = Stopwatch.StartNew();

            manager.StopSubsystems();
            manager.DeinitializeLoader();

            timer.Stop();

            Api = null;

            Debug.Log($"Finished deinitializing, Elapsed {timer.Elapsed.TotalSeconds} seconds");
        }
    }
}
