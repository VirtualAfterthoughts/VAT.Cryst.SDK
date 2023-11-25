using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Cryst.Game
{
    public delegate void TimeUpdate(float current, float previous);

    public static class CrystTimeManager
    {
        #region Variables

        private static int _physicsRate = 50;
        private static float _timeScale = 1f;

        private static bool _isPaused = false;

        #endregion

        #region Properties

        /// <summary>
        /// The amount of times physics ticks are ran per second.
        /// </summary>
        public static int PhysicsRate
        {
            get
            {
                return _physicsRate;
            }
            set
            {
                _physicsRate = value;
                InternalUpdateFixedDeltaTime();
            }
        }

        /// <summary>
        /// The cached and unpaused scale at which time passes.
        /// </summary>
        public static float TimeScale
        {
            get
            {
                return _timeScale;
            }
            set
            {
                // We only update the timescale if the game is unpaused
                // Otherwise, wait until the game is unpaused
                if (!IsPaused)
                {
                    Time.timeScale = value;
                }

                float previous = _timeScale;
                _timeScale = value;

                InternalUpdateFixedDeltaTime();

                OnTimeScaleChanged?.Invoke(value, previous);
            }
        }

        /// <summary>
        /// Is the game currently paused?
        /// </summary>
        public static bool IsPaused
        {
            get
            {
                return _isPaused;
            }
            set
            {
                if (value)
                {
                    Pause();
                }
                else
                {
                    Unpause();
                }
            }
        }

        #endregion

        #region Events

        public static event TimeUpdate OnTimeScaleChanged;

        public static event TimeUpdate OnFixedDeltaChanged;

        #endregion

        #region Functions

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InternalInitializeRuntime()
        {
            // When the game loads, automatically configure the physics rate to the current rate
            PhysicsRate = Mathf.RoundToInt(1f / Time.fixedDeltaTime);
        }

        private static void InternalUpdateFixedDeltaTime()
        {
            var previous = Time.fixedDeltaTime;
            var current = _timeScale / _physicsRate;

            Time.fixedDeltaTime = current;
            OnFixedDeltaChanged?.Invoke(current, previous);
        }
        
        public static void Pause()
        {
            _isPaused = true;
            Time.timeScale = 0f;
        }

        public static void Unpause()
        {
            _isPaused = false;
            Time.timeScale = _timeScale;
        }

        #endregion
    }
}
