using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.LowLevel;

namespace VAT.Cryst.Extensions
{
    public static class PlayerLoopExtensions
    {
        public static int GetSubSystemIndex<T>(this PlayerLoopSystem system) where T : struct
        {
            var type = typeof(T);

            for (var i = 0; i < system.subSystemList.Length; i++)
            {
                if (system.subSystemList[i].type == type)
                {
                    return i;
                }
            }

            return -1;
        }

        public static void AddSubSystem(ref this PlayerLoopSystem system, PlayerLoopSystem systemToAdd)
        {
            List<PlayerLoopSystem> newSubSystemList = new();

            newSubSystemList.AddRange(system.subSystemList);
            newSubSystemList.Add(systemToAdd);

            system.subSystemList = newSubSystemList.ToArray();
        }
    }
}
