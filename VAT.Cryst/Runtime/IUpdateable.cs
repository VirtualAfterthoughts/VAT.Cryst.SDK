using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Cryst
{
    public interface IEarlyUpdateable
    {
        void OnEarlyUpdate(float deltaTime);
    }

    public interface IUpdateable
    {
        void OnUpdate(float deltaTime);
    }

    public interface IFixedUpdateable
    {
        void OnFixedUpdate(float deltaTime);
    }

    public interface ILateUpdateable
    {
        void OnLateUpdate(float deltaTime);
    }
}
