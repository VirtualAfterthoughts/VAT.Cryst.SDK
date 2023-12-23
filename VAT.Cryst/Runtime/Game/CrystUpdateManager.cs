using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.LowLevel;

using VAT.Cryst.Extensions;
using VAT.Cryst.Interfaces;

namespace VAT.Cryst.Game
{
    public static class CrystUpdateManager
    {
        private static readonly HashSet<IEarlyUpdateable> _earlyUpdateables = new();
        private static readonly HashSet<IUpdateable> _updateables = new();
        private static readonly HashSet<IFixedUpdateable> _fixedUpdateables = new();
        private static readonly HashSet<ILateUpdateable> _lateUpdateables = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InternalInitializeRuntime()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

            var earlyUpdate = new PlayerLoopSystem() { updateDelegate = OnEarlyUpdate };
            var update = new PlayerLoopSystem() { updateDelegate = OnUpdate };
            var fixedUpdate = new PlayerLoopSystem() { updateDelegate = OnFixedUpdate };
            var lateUpdate = new PlayerLoopSystem() { updateDelegate = OnLateUpdate };

            playerLoop.subSystemList[playerLoop.GetSubSystemIndex<EarlyUpdate>()].AddSubSystem(earlyUpdate);
            playerLoop.subSystemList[playerLoop.GetSubSystemIndex<Update>()].AddSubSystem(update);
            playerLoop.subSystemList[playerLoop.GetSubSystemIndex<FixedUpdate>()].AddSubSystem(fixedUpdate);
            playerLoop.subSystemList[playerLoop.GetSubSystemIndex<PreLateUpdate>()].AddSubSystem(lateUpdate);

            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        #region Registration

        public static void RegisterEarlyUpdatable(IEarlyUpdateable updatable) => _earlyUpdateables.Add(updatable);
        public static void RegisterUpdatable(IUpdateable updatable) => _updateables.Add(updatable);
        public static void RegisterFixedUpdatable(IFixedUpdateable updatable) => _fixedUpdateables.Add(updatable);
        public static void RegisterLateUpdatable(ILateUpdateable updatable) => _lateUpdateables.Add(updatable);

        #endregion

        #region Unregistration

        public static void UnregisterEarlyUpdatable(IEarlyUpdateable updatable) => _earlyUpdateables.Remove(updatable);
        public static void UnregisterUpdatable(IUpdateable updatable) => _updateables.Remove(updatable);
        public static void UnregisterFixedUpdatable(IFixedUpdateable updatable) => _fixedUpdateables.Remove(updatable);
        public static void UnregisterLateUpdatable(ILateUpdateable updatable) => _lateUpdateables.Remove(updatable);

        #endregion

        #region Update Functions

        private static void OnEarlyUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            float deltaTime = Time.deltaTime;

            foreach (var updatable in _earlyUpdateables)
            {
                updatable.OnEarlyUpdate(deltaTime);
            }
        }

        private static void OnUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            float deltaTime = Time.deltaTime;

            foreach (var updatable in _updateables)
            {
                updatable.OnUpdate(deltaTime);
            }
        }

        private static void OnFixedUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            float deltaTime = Time.deltaTime;

            foreach (var updatable in _fixedUpdateables)
            {
                updatable.OnFixedUpdate(deltaTime);
            }
        }

        private static void OnLateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            float deltaTime = Time.deltaTime;

            foreach (var updatable in _lateUpdateables)
            {
                updatable.OnLateUpdate(deltaTime);
            }
        }

        #endregion
    }
}
