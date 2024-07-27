using UnityEngine;

using VAT.Cryst.Game;

namespace VAT.Logic
{
    public static class LogicManager
    {
        private static LogicUpdateManager _updateManager = null;

        public static LogicUpdateManager UpdateManager => _updateManager;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void OnInitializeRuntime()
        {
            _updateManager = new LogicUpdateManager();

            CrystUpdateManager.RegisterUpdatable(_updateManager);

            Application.quitting += OnApplicationQuit;
        }
        
        private static void OnApplicationQuit()
        {
            CrystUpdateManager.UnregisterUpdatable(_updateManager);
        }
    }
}
